using System;
using System.Linq;
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

        public static bool TryParseExactEnum<T>(this string value, out T t, bool caseInsensitive = true)
            where T : struct =>
                Enum.TryParse(value, caseInsensitive, out t) &&
                Enum.GetNames(typeof(T)).Any(_ => _.Equals(value.Trim(), StringComparison.OrdinalIgnoreCase));
    }
}
