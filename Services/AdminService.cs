using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UserManagementApp.DTOs;
using UserManagementApp.Models;
using Microsoft.Extensions.Logging;

namespace UserManagementApp.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AdminService> _logger;

        public AdminService(UserManager<ApplicationUser> userManager, ILogger<AdminService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            _logger.LogInformation("Fetching all users.");
            return await _userManager.Users
                .OrderByDescending(u => u.LastLoginTime)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    LastLoginTime = u.LastLoginTime,
                    IsBlocked = u.IsBlocked
                })
                .ToListAsync();
        }
        public async Task<IdentityResult> BlockUserAsync(string userId)
        {
            _logger.LogInformation("Attempting to block user with ID: {UserId}", userId);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("BlockUserAsync failed: User with ID {UserId} not found.", userId);
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            user.IsBlocked = true;
            _logger.LogInformation("User with ID {UserId} has been marked as blocked.", userId);
            return await _userManager.UpdateAsync(user);
        }
        public async Task<IdentityResult> UnblockUserAsync(string userId)
        {
            _logger.LogInformation("Attempting to unblock user with ID: {UserId}", userId);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("UnblockUserAsync failed: User with ID {UserId} not found.", userId);
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            user.IsBlocked = false;
            _logger.LogInformation("User with ID {UserId} has been marked as unblocked.", userId);
            return await _userManager.UpdateAsync(user);
        }
        public async Task<IdentityResult> DeleteUserAsync(string userId)
        {
            _logger.LogInformation("Attempting to delete user with ID: {UserId}", userId);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("DeleteUserAsync failed: User with ID {UserId} not found.", userId);
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            _logger.LogInformation("User with ID {UserId} is being deleted.", userId);
            return await _userManager.DeleteAsync(user);
        }
    }
}