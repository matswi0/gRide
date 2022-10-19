using gRide.Data;
using gRide.Models;
using gRide.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gRide.Controllers
{
    public class DashboardController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private const int _numberOfEventsToDisplay = 3;

        public DashboardController(gRideDbContext dbContext, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.EventsHosted)
                .Include(u => u.EventsLinked)
                    .ThenInclude(el => el.Event)
                        .ThenInclude(e => e.Host)
                .Include(u => u.EventsLinked)
                    .ThenInclude(el => el.Event)
                        .ThenInclude(e => e.Localization)
                .SingleOrDefaultAsync(u => u.UserName == User.Identity.Name);

            DashboardViewModel dashboardViewModel = new()
            {
                EventsHosted = appUser.EventsHosted.OrderBy(e => e.Date).Take(_numberOfEventsToDisplay),
                EventsLinked = appUser.EventsLinked.OrderBy(e => e.Event.Date).Take(_numberOfEventsToDisplay)
            };
            
            return View(dashboardViewModel);
        }
    }
}
