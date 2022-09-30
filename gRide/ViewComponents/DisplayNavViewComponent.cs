using gRide.Models;
using gRide.Services;
using gRide.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace gRide.ViewComponents
{
    public class DisplayNavViewComponent : ViewComponent
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserInfo _userInfo;

        public DisplayNavViewComponent(UserManager<AppUser> userManager, IUserInfo userInfo)
        {
            _userManager = userManager;
            _userInfo = userInfo;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!User.Identity.IsAuthenticated)
                return Content(string.Empty);
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            var normalizedUser = _userInfo.NormalizeUser(user.Id, user.UserName, user.ProfilePicture);
            return View(new NavViewModel { UserName = normalizedUser.UserName, ProfilePicture = normalizedUser.ProfilePicture });
        }
    }
}
