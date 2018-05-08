using System.Linq;
using FluentValidation.Results;

namespace Dmarc.Common.Api.Utils
{
    public static class ValidationResultExtensionMethods
    {
        public static string GetErrorString(this ValidationResult validationResult)
        {
            return string.Join(",", validationResult.Errors.Select(_ => $"{_.AttemptedValue ?? "<null>"} is not valid for property {_.PropertyName} : {_.ErrorMessage}"));
        }
    }
}
