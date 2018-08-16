using System.Linq;
using Dmarc.Admin.Api.Domain;
using FluentValidation;

namespace Dmarc.Admin.Api.Validation
{
    public class ChangeMembershipRequestValidator : AbstractValidator<ChangeMembershipRequest>
    {
        public ChangeMembershipRequestValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(r => r.Id)
                .GreaterThan(0)
                .WithMessage("An ID must be greater than zero.");

            RuleFor(r => r.EntityIds)
                .NotEmpty()
                .WithMessage("Entity IDs cannot be empty.")
                .Must(r => r.All(_ => _ > 0))
                .WithMessage("Entity IDs must be greater than zero.");
        }
    }
}
