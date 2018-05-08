using System.Text.RegularExpressions;

namespace Dmarc.Common.Util
{
    public static class StringExtensions
    {
        public static string PascalToSnakeCase(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return Regex.Replace(
                    value,
                    "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])",
                    "_$1",
                    RegexOptions.Compiled)
                .Trim()
                .ToLower();
        }
    }
}
