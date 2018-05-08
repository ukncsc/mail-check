using System;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters
{
    public interface IConverter<out T>
    {
        T Convert(string value, string fieldName, bool parseMandatory);
    }

    public abstract class Converter<T> : IConverter<T>
    {
        private readonly ILogger _log;

        protected Converter(ILogger log)
        {
            _log = log;
        }

        public T Convert(string value, string fieldName, bool parseMandatory)
        {
            T t;
            if (TryConvert(value, out t))
            {
                return t;
            }
            _log.Warn($"Failed to convert {value} to {typeof(T)} for {fieldName}.");
            if (parseMandatory)
            {
                throw new ArgumentException($"Failed to convert {value} to {typeof(T)} for {fieldName}.");
            }

            return OnConvertFailedReturnValue(value, fieldName);
        }

        protected abstract bool TryConvert(string value, out T t);

        protected virtual T OnConvertFailedReturnValue(string value, string fieldName)
        {
            return default(T);
        }
    }
}