using System;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using MimeKit.Utils;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters
{
    public interface IDateTimeConverter : IConverter<DateTime?> { }


    public class DateTimeConverter : Converter<DateTime?>, IDateTimeConverter
    {
        public DateTimeConverter(ILogger log) : base(log) { }

        protected override bool TryConvert(string value, out DateTime? t)
        {
            DateTimeOffset dateTime;
            if (DateUtils.TryParse(value, out dateTime))
            {
                if (dateTime.UtcDateTime == default(DateTime))
                {
                    t = null;
                    return false;
                }
                t = dateTime.UtcDateTime;
                return true;
            }
            t = null;
            return false;
        }
    }
}