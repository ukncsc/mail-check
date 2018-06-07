using Dmarc.DnsRecord.Importer.Lambda.Factory;

namespace Dmarc.DnsRecord.Importer.Lambda
{
    public class MxRecordImporter : DnsRecordImporter
    {
        public MxRecordImporter() : base("Mx", MxRecordProcessorFactory.Create){}
    }
}