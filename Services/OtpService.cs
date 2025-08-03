using Microsoft.AspNetCore.Identity;
using UserManagementApp.Models;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace UserManagementApp.Services
{
    public class OtpService : IOtpService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<OtpService> _logger;

        private static readonly Dictionary<string, string> _otpStore = new Dictionary<string, string>();

        public OtpService(UserManager<ApplicationUser> userManager, IEmailService emailService, ILogger<OtpService> logger)
        {
            _userManager = userManager;
            _emailService = emailService;
            _logger = logger;
        }
        public async Task<bool> GenerateAndSendOtpAsync(string email)
        {
            _logger.LogInformation("Generating OTP for email: {Email}", email);
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("GenerateAndSendOtpAsync called for non-existent email: {Email}", email);
                return true;
            }

            var random = new Random();
            var otp = random.Next(100000, 999999).ToString("D6");

            _otpStore[email] = otp;
            _logger.LogInformation("OTP stored for email: {Email}", email);

            var emailMessage = $"Your password reset code is: {otp}";
            await _emailService.SendEmailAsync(email, "Password Reset (OTP)", emailMessage);

            return true;
        }
        public async Task<string> VerifyOtpAsync(string otp, string email)
        {
            _logger.LogInformation("Verifying OTP for email: {Email}", email);
            if (_otpStore.TryGetValue(email, out string storedOtp) && storedOtp == otp)
            {
                _otpStore.Remove(email);
                _logger.LogInformation("OTP verified successfully for email: {Email}. OTP removed from store.", email);
                return email;
            }
            _logger.LogWarning("OTP verification failed for email: {Email}. Mismatched or expired OTP.", email);
            return null;
        }
    }
}