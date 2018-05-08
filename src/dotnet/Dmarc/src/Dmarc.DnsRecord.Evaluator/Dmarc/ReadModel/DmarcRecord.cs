using System.Collections.Generic;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.ReadModel
{
    public class DmarcRecord
    {
        public DmarcRecord(int index, List<Tag> tags)
        {
            Index = index;
            Tags = tags;
        }

        public int Index { get; }
        public List<Tag> Tags { get; }
    }
}