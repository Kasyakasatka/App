using Microsoft.AspNetCore.Mvc.ViewFeatures;
using UserManagementApp.Models;

namespace UserManagementApp.Services
{
    public interface IOtpService
    {
        Task<bool> GenerateAndSendOtpAsync(string email);
        Task<string> VerifyOtpAsync(string otp, string email);
    }
}
