using System;

namespace Dmarc.MxSecurityTester.Dao.Entities
{
    public class TlsSecurityProfile
    {
        public TlsSecurityProfile(
            ulong? id, 
            DateTime? endDate,
            TlsTestResults tlsResults)
        {
            Id = id;
            EndDate = endDate;
            TlsResults = tlsResults;
        }

        public ulong? Id { get; set; }
        public DateTime? EndDate { get; }
        public TlsTestResults TlsResults { get; }

        protected bool Equals(TlsSecurityProfile other)
        {
            return Id == other.Id && 
                EndDate.Equals(other.EndDate) && 
                Equals(TlsResults, other.TlsResults);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TlsSecurityProfile) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ EndDate.GetHashCode();
                hashCode = (hashCode * 397) ^ (TlsResults?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}