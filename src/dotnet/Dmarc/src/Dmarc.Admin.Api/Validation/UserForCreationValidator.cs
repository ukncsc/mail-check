using Dmarc.Admin.Api.Domain;
using FluentValidation;

namespace Dmarc.Admin.Api.Validation
{
    public class UserForCreationValidator : AbstractValidator<UserForCreation>
    {
        public UserForCreationValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(u => u.FirstName)
                .NotNull()
                .WithMessage("A first name is required.")
                .NotEmpty()
                .WithMessage("A first name cannot be empty.")
                .Matches(@"\A[^\r\n$<>%;/\\]{1,255}\z")
                .WithMessage("A first name cannot contain special characters.");

            RuleFor(u => u.LastName)
                .NotNull()
                .WithMessage("A last name is required.")
                .NotEmpty()
                .WithMessage("A last name cannot be empty.")
                .Matches(@"\A[^\r\n$<>%;/\\]{1,255}\z")
                .WithMessage("A last name cannot contain special characters.");

            RuleFor(u => u.Email)
                .NotNull()
                .WithMessage("An email address is required.")
                .NotEmpty()
                .WithMessage("An email address cannot be empty.")
                .EmailAddress()
                .WithMessage("An email address must be valid.");
        }
    }
}
