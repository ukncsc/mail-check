using Dmarc.Admin.Api.Domain;
using FluentValidation;

namespace Dmarc.Admin.Api.Validation
{
    public class GroupForCreationValidator : AbstractValidator<GroupForCreation>
    {
        public GroupForCreationValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(g => g.Name)
                .NotNull()
                .WithMessage("A name cannot be null.")
                .Matches(@"\A[^\r\n$<>%;/\\]{1,255}\z")
                .WithMessage("A name must not contain special characters.");
        }
    }
}
