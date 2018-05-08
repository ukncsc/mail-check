using Dmarc.Admin.Api.Domain;
using FluentValidation;

namespace Dmarc.Admin.Api.Validation
{
    public class GroupForCreationValidator : AbstractValidator<GroupForCreation>
    {
        public GroupForCreationValidator()
        {
            RuleFor(g => g.Name).NotNull().Matches(@"\A[^\r\n$<>%;/\\]{1,255}\z");
        }
    }
}