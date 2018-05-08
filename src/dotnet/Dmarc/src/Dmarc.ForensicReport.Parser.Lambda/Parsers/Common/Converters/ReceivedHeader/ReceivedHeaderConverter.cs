using System.Collections.Generic;
using System.Net.Mail;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters.ReceivedHeader
{
    public interface IReceivedHeaderConverter : IConverter<Domain.ReceivedHeader> { }

    public class ReceivedHeaderConverter : Converter<Domain.ReceivedHeader>, IReceivedHeaderConverter
    {
        private readonly IReceivedHeaderSplitter _splitter;
        private readonly IMailAddressConverter _mailAddressCollectionConverter;

        public ReceivedHeaderConverter(IReceivedHeaderSplitter splitter, 
            IMailAddressConverter mailAddressCollectionConverter, 
            ILogger log) 
            : base(log)
        {
            _splitter = splitter;
            _mailAddressCollectionConverter = mailAddressCollectionConverter;
        }

        protected override bool TryConvert(string value, out Domain.ReceivedHeader t)
        {
            List<string> parts = _splitter.Split(value);

            string from = GetFields("from", parts);
            string by = GetFields("by", parts);
            string @for = GetFields("for", parts);

            MailAddress forAddresses = @for == null
                ? null
                : _mailAddressCollectionConverter.Convert(@for, string.Empty, false);
            
            t = new Domain.ReceivedHeader(from, by, forAddresses);
            return true;
        }

        private string GetFields(string field, List<string> parts)
        {
            int index = parts.IndexOf(field);

            return index != -1 && parts.Count > index + 1 ? parts[index + 1] : null;
        }
    }
}