using System;
using System.Net.Mail;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters
{
    public interface IMailAddressConverter : IConverter<MailAddress> { }

    public class MailAddressConverter : Converter<MailAddress>, IMailAddressConverter
    {
        public MailAddressConverter(ILogger log) : base(log){}

        protected override bool TryConvert(string value, out MailAddress t)
        {
            try
            {
                t = new MailAddress(value);
                return true;
            }
            catch (Exception)
            {
                t = null;
                return false;
            }
        }
    }
}