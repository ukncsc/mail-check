namespace Dmarc.DnsRecord.Importer.Lambda.Dns.Client.RecordInfos
{
    public class SpfRecordInfo : RecordInfo
    {
        public static SpfRecordInfo EmptyRecordInfo = new SpfRecordInfo(null);

        public SpfRecordInfo(string record)
        {
            Record = record;
        }

        public string Record { get; }


        protected bool Equals(SpfRecordInfo other)
        {
            return string.Equals(Record, other.Record);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SpfRecordInfo) obj);
        }

        public override int GetHashCode()
        {
            return (Record != null ? Record.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return $"{nameof(Record)}: {Record}";
        }
    }
}