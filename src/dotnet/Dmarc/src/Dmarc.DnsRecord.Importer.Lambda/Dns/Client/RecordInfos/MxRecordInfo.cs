namespace Dmarc.DnsRecord.Importer.Lambda.Dns.Client.RecordInfos
{
    public class MxRecordInfo : RecordInfo
    {
        public static MxRecordInfo EmptyRecordInfo = new MxRecordInfo(null);

        private MxRecordInfo(string host)
        {
            Host = host;
            Preference = null;
        }

        public MxRecordInfo(string host, int preference)
        {
            Host = host;
            Preference = preference;
        }

        public string Host { get; }

        public int? Preference { get; }

        protected bool Equals(MxRecordInfo other)
        {
            return string.Equals(Host, other.Host) && Preference == other.Preference;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MxRecordInfo) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Host != null ? Host.GetHashCode() : 0) * 397) ^ Preference.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"{nameof(Host)}: {Host}, {nameof(Preference)}: {Preference}";
        }
    }
}