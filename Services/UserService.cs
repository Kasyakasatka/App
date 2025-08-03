using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using UserManagementApp.Models;
using Microsoft.Extensions.Logging;

namespace UserManagementApp.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserService> _logger;

        public UserService(UserManager<ApplicationUser> userManager, ILogger<UserService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }
        public async Task<ApplicationUser> FindUserByEmailAsync(string email)
        {
            _logger.LogInformation("Attempting to find user by email: {Email}", email);
            return await _userManager.FindByEmailAsync(email);
        }
        public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
        {
            _logger.LogInformation("Attempting to create a new user with email: {Email}", user.Email);
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User created successfully with email: {Email}", user.Email);
            }
            else
            {
                _logger.LogError("User creation failed for email {Email}. Errors: {Errors}", user.Email, string.Join(", ", result.Errors));
            }
            return result;
        }
        public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user)
        {
            _logger.LogInformation("Attempting to update user with ID: {UserId}", user.Id);
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} updated successfully.", user.Id);
            }
            else
            {
                _logger.LogError("User update failed for ID {UserId}. Errors: {Errors}", user.Id, string.Join(", ", result.Errors));
            }
            return result;
        }
        public async Task<IdentityResult> DeleteUserAsync(ApplicationUser user)
        {
            _logger.LogInformation("Attempting to delete user with ID: {UserId}", user.Id);
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} deleted successfully.", user.Id);
            }
            else
            {
                _logger.LogError("User deletion failed for ID {UserId}. Errors: {Errors}", user.Id, string.Join(", ", result.Errors));
            }
            return result;
        }
        public async Task<bool> IsUserBlockedAsync(string email)
        {
            _logger.LogInformation("Checking if user with email {Email} is blocked.", email);
            var user = await _userManager.FindByEmailAsync(email);
            bool isBlocked = user != null && user.IsBlocked;
            _logger.LogInformation("User with email {Email} is blocked: {IsBlocked}", email, isBlocked);
            return isBlocked;
        }
    }
}