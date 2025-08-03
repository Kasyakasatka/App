using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using UserManagementApp.Models;

namespace UserManagementApp.Services
{
    public interface IUserService
    {
        Task<ApplicationUser> FindUserByEmailAsync(string email);
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
        Task<IdentityResult> UpdateUserAsync(ApplicationUser user);
        Task<IdentityResult> DeleteUserAsync(ApplicationUser user);
        Task<bool> IsUserBlockedAsync(string email);
    }
}
