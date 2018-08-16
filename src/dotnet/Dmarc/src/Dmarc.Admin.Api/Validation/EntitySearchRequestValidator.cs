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
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(r => r.Search)
                .Matches(@"\A[^\r\n$<>%;/\\]{0,50}\z")
                .WithMessage("A search must not contain any special characters.");

            RuleFor(r => r.Limit)
                .GreaterThanOrEqualTo(1)
                .WithMessage("A limit must be greater than zero.")
                .LessThanOrEqualTo(200)
                .WithMessage("A limit must be 200 or less.");

            RuleFor(r => r.IncludedIds)
                .Must(r => r.All(_ => _ >= 0))
                .WithMessage("All included IDs must be greater than zero.");
        }
    }
}
