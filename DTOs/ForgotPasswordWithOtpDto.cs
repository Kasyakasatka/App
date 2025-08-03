namespace UserManagementApp.DTOs
{
    public class ForgotPasswordWithOtpDto
    {
        public string Email { get; set; }
        public string Otp { get; set; }
        public string NewPassword { get; set; }
    }
}
