using System;

namespace Dmarc.Common.Util
{
    public static class EnumExtensions
    {
        public static string GetEnumAsString<T>(this T? value) where T : struct =>
            value != null
                ? Enum.GetName(typeof(T), value)
                : "";
    }
}
