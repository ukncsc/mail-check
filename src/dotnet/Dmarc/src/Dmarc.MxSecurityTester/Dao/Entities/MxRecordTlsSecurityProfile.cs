namespace Dmarc.MxSecurityTester.Dao.Entities
{
    public class MxRecordTlsSecurityProfile
    {
        public MxRecordTlsSecurityProfile(MxRecord mxRecord, TlsSecurityProfile tlsSecurityProfile)
        {
            MxRecord = mxRecord;
            TlsSecurityProfile = tlsSecurityProfile;
        }

        public MxRecord MxRecord { get; }

        public TlsSecurityProfile TlsSecurityProfile { get; }

        protected bool Equals(MxRecordTlsSecurityProfile other)
        {
            return Equals(MxRecord, other.MxRecord) && 
                   Equals(TlsSecurityProfile, other.TlsSecurityProfile);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MxRecordTlsSecurityProfile) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((MxRecord?.GetHashCode() ?? 0) * 397) ^ 
                       (TlsSecurityProfile?.GetHashCode() ?? 0);
            }
        }
    }
}