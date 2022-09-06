using gRide.Data;
using gRide.Services;
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
        private readonly IMailSender _mailSender;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            gRideDbContext dbContext, IMailSender mailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
            _mailSender = mailSender;
        }

        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction(nameof(Index), "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
                return View(nameof(Register));

            IdentityUser user = new()
            {
                Email = registerViewModel.Email,
                UserName = registerViewModel.UserName,
            };

            IdentityResult result = await this._userManager.CreateAsync(user, registerViewModel.Password);

            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token }, protocol: Request.Scheme);
                await _mailSender.SendAsync("noreplygRideTeam@gride.com", user.Email,
                    "gRide Team - confirm your email address",
                    $"In order to confirm your email address click on this link: {confirmationLink}");
                return View(nameof(Login));
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    if (error.Code.Contains("Password"))
                        ModelState.AddModelError("Password", error.Description);
                    else if (error.Code.Contains("Email"))
                        ModelState.AddModelError("Email", error.Description);
                    else if (error.Code.Contains("UserName"))
                        ModelState.AddModelError("UserName", error.Description);
                }
                return View(nameof(Register));
            }
        }

        public async Task<IActionResult> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.Message = "Failed to confirm email address";
                return View();
            }

            var confirmCheckResult = await _userManager.IsEmailConfirmedAsync(user);
            if (confirmCheckResult.Equals(true))
                return NotFound();

            var confirmResult = await _userManager.ConfirmEmailAsync(user, token);
            if (!confirmResult.Succeeded)
            {
                ViewBag.Message = "Failed to confirm email address";
                return View();
            }

            ViewBag.Message = "Email address has been successfully confirmed";
            return View();
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction(nameof(Index), "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
                return View();

            var user = await _userManager.FindByEmailAsync(loginViewModel.Email);

            if (user == null)
                return View();

            SignInResult result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, loginViewModel.RemeberMe, false);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Login));
            }
            else
            {
                bool isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
                string message = isEmailConfirmed ? "Invalid log in attempt" : "In order to log in please confirm your email address";
                ModelState.AddModelError(string.Empty, message);
                return View(nameof(Login));
            }
        }

        [Authorize]
        public async Task<IActionResult> LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index), "Home");
        }

        public IActionResult ForgotPassword()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction(nameof(Index), "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordViewModel.Email);

            if (!ModelState.IsValid)
                return View();
            if (user == null || !user.EmailConfirmed)
            {
                ModelState.AddModelError("Email", "There is no account associated with this email address");
                return View(nameof(ForgotPassword));
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResetLink = Url.Action("ResetPassword", "Account", new { userId = user.Id, token }, protocol: Request.Scheme);
            await _mailSender.SendAsync("noreplygRideTeam@gride.com", user.Email,
                "gRide Team - password reset",
                $"In order to reset your password click on this link: {passwordResetLink}");
            return RedirectToAction(nameof(Index), "Home");
        }

        public async Task<IActionResult> ResetPasswordAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var isTokenValid = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, 
                "ResetPassword", token);
            if (User.Identity.IsAuthenticated || !isTokenValid)
                return NotFound();

            return View(new ResetPasswordViewModel { Token = token, UserId = userId });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordViewModel resetPasswordViewModel)
        {
            if (!ModelState.IsValid)
                return View(resetPasswordViewModel);

            IdentityUser user = await _userManager.FindByIdAsync(resetPasswordViewModel.UserId);
            if(user == null)
                return RedirectToAction(nameof(Index), "Home");

            IdentityResult result = await _userManager.ResetPasswordAsync(user, resetPasswordViewModel.Token, resetPasswordViewModel.Password);
            if(!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }
            return View(nameof(Login));
        }
    }
}
