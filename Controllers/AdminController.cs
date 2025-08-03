using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementApp.Models;
using UserManagementApp.Services;
using System.Threading.Tasks;
using System.Linq;
using UserManagementApp.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity; 

namespace UserManagementApp.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IAuthenticationService _authService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, IAuthenticationService authService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _authService = authService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Admin dashboard requested.");
            var users = await _adminService.GetAllUsersAsync();

            var dto = new UserManagementDto
            {
                Users = users
            };

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Block(string[] selectedUsers)
        {
            _logger.LogInformation("Block user action requested.");
            if (selectedUsers == null || selectedUsers.Length == 0)
            {
                _logger.LogWarning("Attempt to block users with no selections.");
                TempData["StatusMessage"] = "Please select at least one user.";
                return RedirectToAction("Index");
            }

            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var isCurrentUserBlocked = false;

            foreach (var userId in selectedUsers)
            {
                await _adminService.BlockUserAsync(userId);
                _logger.LogInformation("User with ID {UserId} was blocked.", userId);
                if (userId == currentUserId)
                {
                    isCurrentUserBlocked = true;
                }
            }

            if (isCurrentUserBlocked)
            {
                await _authService.LogoutAsync();
                return RedirectToAction("Login", "Account");
            }

            TempData["StatusMessage"] = "Selected users have been blocked.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Unblock(string[] selectedUsers)
        {
            _logger.LogInformation("Unblock user action requested.");
            if (selectedUsers == null || selectedUsers.Length == 0)
            {
                _logger.LogWarning("Attempt to unblock users with no selections.");
                TempData["StatusMessage"] = "Please select at least one user.";
                return RedirectToAction("Index");
            }

            foreach (var userId in selectedUsers)
            {
                await _adminService.UnblockUserAsync(userId);
                _logger.LogInformation("User with ID {UserId} was unblocked.", userId);
            }

            TempData["StatusMessage"] = "Selected users have been unblocked.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string[] selectedUsers)
        {
            _logger.LogInformation("Delete user action requested.");
            if (selectedUsers == null || selectedUsers.Length == 0)
            {
                _logger.LogWarning("Attempt to delete users with no selections.");
                TempData["StatusMessage"] = "Please select at least one user.";
                return RedirectToAction("Index");
            }

            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var isCurrentUserDeleted = false;

            foreach (var userId in selectedUsers)
            {
                await _adminService.DeleteUserAsync(userId);
                _logger.LogInformation("User with ID {UserId} was deleted.", userId);
                if (userId == currentUserId)
                {
                    isCurrentUserDeleted = true;
                }
            }

            if (isCurrentUserDeleted)
            {
                await _authService.LogoutAsync();
                return RedirectToAction("Login", "Account");
            }

            TempData["StatusMessage"] = "Selected users have been deleted.";
            return RedirectToAction("Index");
        }
    }
}
