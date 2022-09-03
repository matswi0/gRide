using gRide.Data;
using gRide.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
            if (User.Identity.IsAuthenticated) return RedirectToAction(nameof(Index) , "Home");
            else return View();
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
                return RedirectToAction(nameof(Index), "Home");
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
            if (User.Identity.IsAuthenticated) return RedirectToAction(nameof(Index), "Home");
            else return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid) return View();

            var user = _dbContext.Users.FirstOrDefault(u => u.Email == loginViewModel.Email);
            if (user == null) return View();

            SignInResult result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, loginViewModel.RemeberMe, false);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Login));
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid log in attempt");
                return View(nameof(Login));
            }
        }

        [Authorize]
        public async Task<IActionResult> LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index), "Home");
        }
    }
}
