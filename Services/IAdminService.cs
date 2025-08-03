using Microsoft.AspNetCore.Identity;
using UserManagementApp.DTOs;
using UserManagementApp.Models;

namespace UserManagementApp.Services
{
    public interface IAdminService
    {
        Task<List<UserDto>> GetAllUsersAsync();
        Task<IdentityResult> BlockUserAsync(string userId);
        Task<IdentityResult> UnblockUserAsync(string userId);
        Task<IdentityResult> DeleteUserAsync(string userId);
    }
}
