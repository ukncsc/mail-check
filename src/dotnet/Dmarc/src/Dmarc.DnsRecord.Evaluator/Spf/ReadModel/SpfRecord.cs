using System.Collections.Generic;

namespace Dmarc.DnsRecord.Evaluator.Spf.ReadModel
{
    public class SpfRecord
    {
        public SpfRecord(int index, Version version, List<Term> terms)
        {
            Index = index;
            Version = version;
            Terms = terms;
        }

        public int Index { get; }
        public Version Version { get; }
        public List<Term> Terms { get; }
    }
}