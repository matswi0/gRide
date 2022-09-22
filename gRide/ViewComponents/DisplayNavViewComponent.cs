using gRide.Data;
using gRide.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace gRide.ViewComponents
{
    public class DisplayNavViewComponent : ViewComponent
    {
        private readonly UserManager<AppUser> _userManager;

        public DisplayNavViewComponent(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!User.Identity.IsAuthenticated)
                return Content(string.Empty);
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            string byteArrayToBase64 = Convert.ToBase64String(user.ProfilePicture);
            string imgDataURL = $"data:image/png;base64,{byteArrayToBase64}";
            return View(new NavViewModel { UserName = user.UserName, ProfilePicture = imgDataURL });
        }
    }
}
