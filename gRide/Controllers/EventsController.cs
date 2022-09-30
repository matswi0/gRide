using FluentValidation;
using FluentValidation.Results;
using gRide.Data;
using gRide.FluentValidation;
using gRide.Models;
using gRide.Services;
using gRide.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace gRide.Controllers
{
    public class EventsController : Controller
    {
        private readonly gRideDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserInfo _userInfo;
        private readonly IValidator<NewEventViewModel> _eventValidator;
        private readonly IViewConverter _viewConverter;

        public EventsController(gRideDbContext dbContext, UserManager<AppUser> userManager, IUserInfo userInfo, 
            IValidator<NewEventViewModel> eventValidator, IViewConverter viewConverter)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _userInfo = userInfo;
            _eventValidator = eventValidator;
            _viewConverter = viewConverter;
        }
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NewEventAsync([FromBody] NewEventViewModel newEventViewModel)
        {
            ValidationResult result = await _eventValidator.ValidateAsync(newEventViewModel);
            if (!result.IsValid)
            {
                result.AddToModelState(ModelState);
                return PartialView("_EventForm", newEventViewModel);
            }
            AppUser user = await _userManager.GetUserAsync(User);

            var uniqueEvent = await _dbContext.Events.AnyAsync(e => e.Name == newEventViewModel.Name && e.Host.Id == user.Id && e.Date == newEventViewModel.Date);
            if (uniqueEvent)
            {
                ModelState.AddModelError(string.Empty, "You have already created an event with this name and date");
                return PartialView("_EventForm", newEventViewModel);
            }

            Localization localization = new()
            {
                Id = Guid.NewGuid(),
                Latitude = newEventViewModel.Latitude,
                Longitude = newEventViewModel.Longitude,
                City = newEventViewModel.Town,
            };

            Event evt = new()
            {
                Id = Guid.NewGuid(),
                Name = newEventViewModel.Name,
                Date = newEventViewModel.Date,
                Host = user,
                Localization = localization,
            };

            AppUserEvent appUserEvent = new()
            {
                UserId = user.Id,
                EventId = evt.Id,
                UserStatus = UserStatus.Going
            };

            await _dbContext.AppUsersEvents.AddAsync(appUserEvent);
            await _dbContext.Events.AddAsync(evt);
            var addEventResul = await _dbContext.SaveChangesAsync();

            user.EventsHosted.Add(evt);
            var updateUserEventsResult = await _userManager.UpdateAsync(user);
            if (!updateUserEventsResult.Succeeded)
                return BadRequest(new { message = updateUserEventsResult.Errors.FirstOrDefault().Description });

            PartialViewResult partialViewResult = PartialView("_InviteFriends", _userInfo.NormalizeFriendList(user));

            return Json(new
            {
                data = _viewConverter.ConvertViewToString(this.ControllerContext, partialViewResult),
                eventId = evt.Id,
                redirectUrl = Url.Action("Index", "Dashboard")
            });
        }

        [HttpPost]
        public async Task<IActionResult> InviteFriendsAsync(Guid id, [FromBody] ICollection<Guid> friendsList)
        {
            Event evt = _dbContext.Events.Where(e => e.Id.Equals(id)).Include(e => e.UsersLinked).FirstOrDefault();
            if (evt == null) 
                return BadRequest();

            foreach (var friendId in friendsList)
            {
                AppUserEvent ue = new()
                {
                    EventId = id,
                    UserId = friendId,
                    UserStatus = UserStatus.Invited
                };
                evt.UsersLinked.Add(ue);
            }

            await _dbContext.SaveChangesAsync();

            return Json(new
            {
                redirectUrl = Url.Action("DisplayEvent", "Events", new { id = evt.Id })
            });
        }

        [Authorize]
        public async Task<IActionResult> DisplayEvent(Guid id)
        {
            Event evt = _dbContext.Events.Where(e => e.Id.Equals(id)).Include(e => e.Localization).Include(e => e.UsersLinked).ThenInclude(ul => ul.User).FirstOrDefault();
            if(evt == null)
                return NotFound();
            return View(evt);
        }
    }
}
