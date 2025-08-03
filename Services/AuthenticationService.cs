using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using UserManagementApp.Models;
using UserManagementApp.DTOs;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace UserManagementApp.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AuthenticationService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }
        public async Task<bool> RegisterUserAsync(RegisterUserDto dto)
        {
            _logger.LogInformation("Attempting to register new user with email: {Email}", dto.Email);
            var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email, Name = dto.Name, LastLoginTime = DateTime.UtcNow };
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with email {Email} registered successfully.", dto.Email);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return true;
            }

            _logger.LogError("User registration failed for email {Email}. Errors: {Errors}", dto.Email, string.Join(", ", result.Errors.Select(e => e.Description)));

            return false;
        }
        public async Task<SignInResult> PasswordSignInAsync(LoginUserDto dto)
        {
            _logger.LogInformation("Password sign-in attempt for email: {Email}", dto.Email);
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user != null && user.IsBlocked)
            {
                _logger.LogWarning("Sign-in failed for user {Email}: account is blocked.", dto.Email);
                return SignInResult.Failed;
            }

            var result = await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {Email} signed in successfully.", dto.Email);
                user.LastLoginTime = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
            }
            else
            {
                _logger.LogWarning("Password sign-in failed for user {Email}.", dto.Email);
            }

            return result;
        }
        public async Task LogoutAsync()
        {
            _logger.LogInformation("User is logging out.");
            await _signInManager.SignOutAsync();
        }
    }
}