using gRide.Data;
using gRide.Services;
using gRide.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Security.Claims;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace gRide.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly gRideDbContext _dbContext;
        private readonly IMailSender _mailSender;
        private readonly IWebHostEnvironment _env;
        private const string defaultProfilePicture = "profile_picture.png";

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            gRideDbContext dbContext, IMailSender mailSender, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
            _mailSender = mailSender;
            _env = env;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
                return View("Views/Home/Index.cshtml");

            byte[] profile_picture;
            try
            {
                string path = Path.Combine(_env.WebRootPath, "img", defaultProfilePicture);
                profile_picture = await System.IO.File.ReadAllBytesAsync(path);
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home");
            }

            AppUser user = new()
            {
                Email = registerViewModel.Email,
                UserName = registerViewModel.UserName,
                ProfilePicture = profile_picture
            };

            IdentityResult result = await _userManager.CreateAsync(user, registerViewModel.Password);
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
                return View("Views/Home/Index.cshtml");
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
                return RedirectToAction(nameof(Index), "Dashboard");
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
                return RedirectToAction(nameof(Index), "Dashboard");
            }
            else
            {
                bool isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
                string message = isEmailConfirmed ? "Invalid log in attempt" : "In order to log in please confirm your email address";
                ModelState.AddModelError(string.Empty, message);
                return View("Login");
            }
        }

        [HttpPost]
        public IActionResult ExternalLogin(string provider)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        public async Task<IActionResult> ExternalLoginCallbackAsync(string provider)
        {
            var loginInfo = await _signInManager.GetExternalLoginInfoAsync();
            var emailClaim = loginInfo.Principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
            var nameClaim = loginInfo.Principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);

            if(emailClaim == null && nameClaim == null)
                return RedirectToAction(nameof(Index), "Home");

            AppUser user = new()
            {
                Email = emailClaim.Value,
                UserName = nameClaim.Value
            };

            await _signInManager.SignInAsync(user, false);
            return RedirectToAction(nameof(Index), "Home");
        }

        [Authorize]
        public async Task<IActionResult> LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        public IActionResult ForgotPasswordPartial()
        {
            return PartialView("_ForgotPassword");
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordViewModel.Email);

            if (!ModelState.IsValid)
                return View("Views/Account/Login.cshtml");
            if (user == null || !user.EmailConfirmed)
            {
                ViewBag.Message = "There is no account associated with this email address";
                return RedirectToAction("Login");
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

            AppUser user = await _userManager.FindByIdAsync(resetPasswordViewModel.UserId);
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
            return View("Login");
        }
    }
}
