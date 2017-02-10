using System.Threading;
using System.Threading.Tasks;
using Dmarc.AggregateReport.Api.Messages;
using FluentValidation.Results;

namespace Dmarc.AggregateReport.Api.Validation
{
    internal interface IValidator<in T>
        where T : Request
    {
        Task<ValidationResult> ValidateAsync(T t, CancellationToken cancellationToken = default(CancellationToken));
    }
}