using System;
using System.Reflection;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters
{
    public interface IEnumConverter<T> : IConverter<T?> where T : struct {}

    public abstract class EnumConverter<T> : Converter<T?>, IEnumConverter<T> where T : struct
    {
        protected EnumConverter(ILogger log) : base(log){}
        protected override bool TryConvert(string value, out T? t)
        {
            if (!typeof(T).GetTypeInfo().IsEnum)
            {
                throw new ArgumentException($"{typeof(T)} is not an Enum");
            }

            T tValue;
            if (Enum.TryParse(value, true, out tValue))
            {
                t = tValue;
                return true;
            }
            t = null;
            return false;
        }
    }
}