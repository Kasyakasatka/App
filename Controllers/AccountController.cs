using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserManagementApp.Models;
using System.Threading.Tasks;
using UserManagementApp.Services;
using UserManagementApp.DTOs;
using Microsoft.Extensions.Logging;

namespace UserManagementApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthenticationService _authService;
        private readonly IOtpService _otpService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAuthenticationService authService, IOtpService otpService, UserManager<ApplicationUser> userManager, ILogger<AccountController> logger)
        {
            _authService = authService;
            _otpService = otpService;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            _logger.LogInformation("Forgot password page requested.");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            _logger.LogInformation("Forgot password request for email: {Email}", dto.Email);
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for forgot password request for email: {Email}", dto.Email);
                return View(dto);
            }

            await _otpService.GenerateAndSendOtpAsync(dto.Email);
            _logger.LogInformation("OTP generated and sent to email: {Email}", dto.Email);

            TempData["otp_email"] = dto.Email;
            return RedirectToAction("VerifyOtp");
        }

        [HttpGet]
        public IActionResult VerifyOtp()
        {
            _logger.LogInformation("OTP verification page requested.");
            ViewBag.Email = TempData["otp_email"] as string;
            if (string.IsNullOrEmpty(ViewBag.Email))
            {
                _logger.LogWarning("Email not found in TempData for OTP verification. Redirecting to ForgotPassword.");
                return RedirectToAction("ForgotPassword");
            }
            TempData.Keep("otp_email");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(ForgotPasswordWithOtpDto model)
        {
            _logger.LogInformation("OTP verification attempt for email: {Email}", model.Email);
            var verifiedEmail = await _otpService.VerifyOtpAsync(model.Otp, model.Email);

            if (verifiedEmail != null)
            {
                _logger.LogInformation("OTP verified successfully for email: {Email}", verifiedEmail);
                var user = await _userManager.FindByEmailAsync(verifiedEmail);
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Password reset successful for user: {UserId}", user.Id);
                        return RedirectToAction("Login");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                        _logger.LogError("Password reset failed for user {UserId} with error: {ErrorDescription}", user.Id, error.Description);
                    }
                }
            }

            _logger.LogWarning("OTP verification failed for email: {Email}", model.Email);
            ModelState.AddModelError(string.Empty, "Invalid OTP, email or password.");
            ViewBag.Email = model.Email;
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            _logger.LogInformation("Registration page requested.");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserDto dto)
        {
            _logger.LogInformation("Registration attempt for email: {Email}", dto.Email);
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for registration attempt for email: {Email}", dto.Email);
                return View(dto);
            }

            if (await _authService.RegisterUserAsync(dto))
            {
                _logger.LogInformation("User registered successfully with email: {Email}", dto.Email);
                return RedirectToAction("Index", "Admin");
            }

            _logger.LogError("Registration failed for email: {Email}. User may already exist.", dto.Email);
            ModelState.AddModelError(string.Empty, "Registration failed. A user with this email may already exist.");
            return View(dto);
        }

        [HttpGet]
        public IActionResult Login()
        {
            _logger.LogInformation("Login page requested.");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user != null && user.IsBlocked)
            {
                ModelState.AddModelError(string.Empty, "Your account is blocked.");
                return View(dto);
            }

            var result = await _authService.PasswordSignInAsync(dto);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Admin");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "The account is temporarily locked.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login or password.");
            }

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("User logout requested.");
            await _authService.LogoutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}