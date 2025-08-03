using FluentValidation;
using UserManagementApp.Models;
using System;

namespace UserManagementApp.Validators
{
    public class RefreshTokenValidator : AbstractValidator<RefreshToken>
    {
        public RefreshTokenValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Token value cannot be empty.");

            RuleFor(x => x.ExpirationDate)
                .NotEmpty().WithMessage("Token expiration date is required.")
                .GreaterThan(DateTime.UtcNow).WithMessage("The token has already expired.");

            RuleFor(x => x.IsRevoked)
                .Equal(false).WithMessage("The token has been revoked.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");
        }
    }
}