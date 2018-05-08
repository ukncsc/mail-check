using Dmarc.Admin.Api.Domain;
using FluentValidation;

namespace Dmarc.Admin.Api.Validation
{
    public class UserForCreationValidator : AbstractValidator<UserForCreation>
    {
        public UserForCreationValidator()
        {
            RuleFor(u => u.FirstName).NotNull().Matches(@"\A[^\r\n$<>%;/\\]{1,255}\z");

            RuleFor(u => u.LastName).NotNull().Matches(@"\A[^\r\n$<>%;/\\]{1,255}\z");

            RuleFor(u => u.Email).NotNull().EmailAddress();
        }
    }
}