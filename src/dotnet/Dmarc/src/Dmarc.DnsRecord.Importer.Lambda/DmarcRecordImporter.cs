using Dmarc.DnsRecord.Importer.Lambda.Factory;

namespace Dmarc.DnsRecord.Importer.Lambda
{
    public class DmarcRecordImporter : DnsRecordImporter
    {
        public DmarcRecordImporter() : base("Dmarc", DmarcRecordProcessorFactory.Create){}
    }
}
