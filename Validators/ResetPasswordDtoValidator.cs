using FluentValidation;
using UserManagementApp.DTOs;

namespace UserManagementApp.Validators
{
    public class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
    {
        public ResetPasswordDtoValidator()
        {
            RuleFor(dto => dto.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(dto => dto.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .MaximumLength(100).WithMessage("Password cannot exceed 100 characters.");

            RuleFor(dto => dto.ConfirmPassword)
                .NotEmpty().WithMessage("Password confirmation is required.")
                .Equal(dto => dto.Password).WithMessage("The password and confirmation password do not match.");

            RuleFor(dto => dto.Token)
                .NotEmpty().WithMessage("The password reset token is required.");
        }
    }
}