using Dmarc.Admin.Api.Domain;
using Dmarc.Common.Validation;
using FluentValidation;

namespace Dmarc.Admin.Api.Validation
{
    public class DomainForCreationValidator : AbstractValidator<DomainForCreation>
    {
        public DomainForCreationValidator(IDomainValidator domainValidator)
        {
            RuleFor(d => d.Name)
                .Must(domainValidator.IsValidDomain)
                .WithMessage("A name must be a valid domain name.");
        }
    }
}
