using System;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using Dmarc.Admin.Api.Domain;
using FluentValidation;

namespace Dmarc.Admin.Api.Validation
{
    public class EntitySearchRequestValidator : AbstractValidator<EntitySearchRequest>
    {
        public EntitySearchRequestValidator()
        {
            RuleFor(r => r.Search).Matches(@"\A[^\r\n$<>%;/\\]{0,50}\z");

            RuleFor(r => r.Limit).GreaterThanOrEqualTo(1).LessThanOrEqualTo(200);

            RuleFor(r => r.IncludedIds).Must(r => r.All(_ => _ >= 0));
        }
    }
}