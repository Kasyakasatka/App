using FluentValidation;
using UserManagementApp.DTOs;

namespace UserManagementApp.Validators
{
    public class ForgotPasswordWithOtpDtoValidator : AbstractValidator<ForgotPasswordWithOtpDto>
    {
        public ForgotPasswordWithOtpDtoValidator()
        {
            RuleFor(dto => dto.Email)
                .NotEmpty().WithMessage("Please enter your email address.")
                .EmailAddress().WithMessage("Please enter a valid email address.");

            RuleFor(dto => dto.Otp)
                .NotEmpty().WithMessage("OTP is required.")
                .Length(6).WithMessage("OTP must be 6 digits long.")
                .Matches(@"^\d{6}$").WithMessage("OTP must consist of 6 digits only.");

            RuleFor(dto => dto.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(6).WithMessage("New password must be at least 6 characters long.")
                .MaximumLength(100).WithMessage("New password cannot exceed 100 characters.");
        }
    }
}