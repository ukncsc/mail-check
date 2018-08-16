using Dmarc.Admin.Api.Domain;
using Dmarc.Common.Validation;
using FluentValidation;

namespace Dmarc.Admin.Api.Validation
{
    public class PublicDomainValidator : AbstractValidator<PublicDomainForCreation>
    {
        public PublicDomainValidator(IDomainValidator domainValidator, IPublicDomainListValidator publicDomainValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(d => d.Name)
                .NotNull()
                .WithMessage("A name is required.")
                .NotEmpty()
                .WithMessage("A name cannot be empty.")
                .Must(domainValidator.IsValidDomain)
                .WithMessage("A name must be a valid domain.")
                .Must(publicDomainValidator.IsValidPublicDomain)
                .WithMessage("A name must be a public domain name and cannot be a top level domain (TLD).");
        }
    }
}
