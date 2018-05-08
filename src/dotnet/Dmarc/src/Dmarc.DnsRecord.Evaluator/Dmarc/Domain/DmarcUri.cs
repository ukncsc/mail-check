using System;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Domain
{
    public class DmarcUri : DmarcEntity
    {
        public DmarcUri(Uri uri)
        {
            Uri = uri;
        }

        public Uri Uri { get; }

        public override string ToString()
        {
            return $"{nameof(Uri)}: {Uri}";
        }
    }
}