using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Dmarc.Common.Linq;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.Rfc822.Headers
{
    public interface IMailAddressCollectionParserMulti : IHeaderParser<MailAddressCollection> { }

    public class MailAddressCollectionParserMulti : HeaderParser<MailAddressCollection>, IMailAddressCollectionParserMulti
    {
        private readonly IMailAddressCollectionConverter _converter;

        public MailAddressCollectionParserMulti(IMailAddressCollectionConverter converter)
        {
            _converter = converter;
        }

        protected override MailAddressCollection DoConvert(List<string> values, string fieldName, bool valueMandatory, bool parseMandatory)
        {
            if (valueMandatory && !values.Any())
            {
                throw new ArgumentException($"Expected {fieldName} to have at least 1 value but had none.");
            }

            List<MailAddressCollection> internetAddressLists = values
                .Select(_ => _converter.Convert(_, fieldName, parseMandatory))
                .Where(_ => _ != null)
                .ToList();

            MailAddressCollection mailAddressCollection = new MailAddressCollection();
            internetAddressLists.SelectMany(_ => _).ForEach(mailAddressCollection.Add);
            return mailAddressCollection;
        }
    }
}