using Dmarc.Admin.Api.Domain;
using FluentValidation;

namespace Dmarc.Admin.Api.Validation
{
    public class GetEntitiesByRelatedIdRequestValidator : AbstractValidator<GetEntitiesByRelatedIdRequest>
    {
        public GetEntitiesByRelatedIdRequestValidator()
        {
            RuleFor(r => r.Id).GreaterThanOrEqualTo(0);

            RuleFor(dr => dr.Search).Matches(@"\A[^\r\n$<>%;/\\]{0,50}\z");

            RuleFor(r => r.Page).GreaterThanOrEqualTo(1);

            RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(200);
        }
    }
}