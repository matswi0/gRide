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
        public async Task <IActionResult> UpdatePictureAsync(EditProfileViewModel editProfileViewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("ProfilePicture", "Failed to update profile picture");
                return View(nameof(Index));
            }

            AppUser user = await _userManager.GetUserAsync(User);
            using (MemoryStream ms = new())
            {
                await editProfileViewModel.ProfilePicture.CopyToAsync(ms);
                user.ProfilePicture = ms.ToArray();
            }
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return RedirectToAction("Index", "Settings");
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("ProfilePicture", error.Description);
                }
                return View(nameof(Index));
            }
        }
    }
}
