using FluentValidation;
using UserManagementApp.Models;
using System;

namespace UserManagementApp.Validators
{
    public class ApplicationUserValidator : AbstractValidator<ApplicationUser>
    {
        public ApplicationUserValidator()
        {
            RuleFor(user => user.Name)
                .NotEmpty().WithMessage("Name cannot be empty.")
                .MaximumLength(100).WithMessage("Name cannot be longer than 100 characters.");

            RuleFor(user => user.LastLoginTime)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Last login time cannot be in the future.");

            RuleFor(user => user.IsBlocked)
                .Must(isBlocked => !isBlocked).When(user => user.Id == null)
                .WithMessage("Cannot register a blocked user.");
        }
    }
}