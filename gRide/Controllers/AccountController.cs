using gRide.Data;
using gRide.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace gRide.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly gRideDbContext _dbContext;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, gRideDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid) return View(nameof(Register));

            IdentityUser user = new()
            {
                Email = registerViewModel.Email,
                UserName = registerViewModel.UserName,
            };

            IdentityResult result = await this._userManager.CreateAsync(user, registerViewModel.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("HomeController");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    if (error.Code.Contains("Password"))
                    {
                        ModelState.AddModelError("Password", error.Description);
                    }
                    else if (error.Code.Contains("Email"))
                    {
                        ModelState.AddModelError("Email", error.Description);
                    }
                    else if (error.Code.Contains("UserName"))
                    {
                        ModelState.AddModelError("UserName", error.Description);
                    }
                }
                return View(nameof(Register));
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(AuthViewModel authViewModel)
        {
            if (!ModelState.IsValid) return View();

            var user = _dbContext.Users.FirstOrDefault(u => u.Email == authViewModel.Email);
            if (user == null) return View();

            SignInResult result = await _signInManager.PasswordSignInAsync(user, authViewModel.Password, true, false);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Login));
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                return View(nameof(Login));
            }
        }

        public async Task<IActionResult> LogoutAsync()
        {
            if (!User.Identity.IsAuthenticated) return View(nameof(Login));

            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }
    }
}
