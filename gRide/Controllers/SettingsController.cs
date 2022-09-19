using gRide.Data;
using gRide.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace gRide.Controllers
{
    public class SettingsController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public SettingsController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task <IActionResult> ChangePictureAsync(EditProfileViewModel editProfileViewModel)
        {
            AppUser user = await _userManager.GetUserAsync(User);
            using (MemoryStream ms = new())
            {
                var memoryStream = new MemoryStream();
                await editProfileViewModel.ProfilePicture.CopyToAsync(memoryStream);
                user.ProfilePicture = memoryStream.ToArray();
            }
            await _userManager.UpdateAsync(user);
            return View(nameof(Index));
        }
    }
}
