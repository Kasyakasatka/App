using Microsoft.AspNetCore.Identity;
using UserManagementApp.DTOs;
using UserManagementApp.Models;

namespace UserManagementApp.Services
{
    public interface IAuthenticationService
    {
        Task<bool> RegisterUserAsync(RegisterUserDto dto);
        Task<SignInResult> PasswordSignInAsync(LoginUserDto dto);
        Task LogoutAsync();
    }
}
