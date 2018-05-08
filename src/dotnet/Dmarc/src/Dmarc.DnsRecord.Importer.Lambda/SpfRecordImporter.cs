using Dmarc.DnsRecord.Importer.Lambda.Factory;

namespace Dmarc.DnsRecord.Importer.Lambda
{
    public class SpfRecordImporter : DnsRecordImporter
    {
        public SpfRecordImporter() : base("Spf", DnsRecordProcessorFactory.CreateSpfProcessor){ }
    }
}
