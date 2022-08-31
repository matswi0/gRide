using gRide.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace gRide.Controllers
{
    public class RegisterController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public RegisterController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid) return View(nameof(Index));

            IdentityUser user = new ()
            {
                Email = registerViewModel.Email,
                UserName = registerViewModel.Email,
            };

            IdentityResult result = await this._userManager.CreateAsync(user, registerViewModel.Password);

            if (result.Succeeded)
            { 
                return RedirectToAction(nameof(Index));
            }
            else 
            {
                foreach (var error in result.Errors)
                {
                    if (error.Code.Contains("Password"))
                    {
                        ModelState.AddModelError("Password", error.Description);
                    }
                    else
                    {
                        ModelState.AddModelError("Email", error.Description);
                    }
                }
                return View(nameof(Index));
            }    
        }
    }
}
