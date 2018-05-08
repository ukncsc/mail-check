using System.Linq;
using Dmarc.Admin.Api.Domain;
using FluentValidation;

namespace Dmarc.Admin.Api.Validation
{
    public class ChangeMembershipRequestValidator : AbstractValidator<ChangeMembershipRequest>
    {
        public ChangeMembershipRequestValidator()
        {
            RuleFor(r => r.Id).GreaterThan(0);

            RuleFor(r => r.EntityIds)
                .NotEmpty()
                .Must(r => r.All(_ => _ > 0));
        }
    }
}