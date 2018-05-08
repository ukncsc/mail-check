using Dmarc.Admin.Api.Domain;
using FluentValidation;

namespace Dmarc.Admin.Api.Validation
{
    public class AllEntitiesSearchRequestValidator : AbstractValidator<AllEntitiesSearchRequest>
    {
        public AllEntitiesSearchRequestValidator()
        {
            RuleFor(r => r.Search).Matches(@"\A[^\r\n$<>%;/\\]{0,50}\z");
            RuleFor(r => r.Limit).GreaterThanOrEqualTo(1).LessThanOrEqualTo(200);
        }   
    }
}