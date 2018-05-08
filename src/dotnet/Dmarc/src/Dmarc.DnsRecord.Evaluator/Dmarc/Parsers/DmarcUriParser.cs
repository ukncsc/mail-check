using System;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Parsers
{
    public interface IDmarcUriParser
    {
        DmarcUri Parse(string value);
    }

    public class DmarcUriParser : IDmarcUriParser
    {
        public DmarcUri Parse(string value)
        {
            Uri uri = value != null && Uri.IsWellFormedUriString(value, UriKind.Absolute)
                ? new Uri(value, UriKind.Absolute)
                : null;

            DmarcUri dmarcUri = new DmarcUri(uri);

            if (uri == null)
            {
                string errorMessage = string.Format(DmarcParserResource.InvalidValueErrorMessage, "uri", value);
                dmarcUri.AddError(new Error(ErrorType.Error, errorMessage));
            }

            return dmarcUri;
        }
    }
}