using Dmarc.DnsRecord.Importer.Lambda.Util;

namespace Dmarc.DnsRecord.Importer.Lambda.Dns.Client.RecordInfos
{
    public class DmarcRecordInfo : RecordInfo
    {
        public static DmarcRecordInfo EmptyRecordInfo = new DmarcRecordInfo(null); 

        public DmarcRecordInfo(string record)
        {
            Record = record;
        }

        public string Record { get; }

        protected bool Equals(DmarcRecordInfo other)
        {
            return StringUtils.SpaceInsensitiveEquals(Record, other.Record);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DmarcRecordInfo) obj);
        }

        public override int GetHashCode()
        {
            return Record?.GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            return $"{nameof(Record)}: {Record}";
        }
    }
}