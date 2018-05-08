namespace Dmarc.MxSecurityTester.Dao.Entities
{
    public class MxRecord
    {
        public MxRecord(
            ulong id,
            string hostname)
        {
            Id = id;
            Hostname = hostname;
        }

        public ulong Id { get; }
        public string Hostname { get; }

        protected bool Equals(MxRecord other)
        {
            return Id == other.Id && 
                string.Equals(Hostname, other.Hostname);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MxRecord) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id.GetHashCode() * 397) ^ 
                    (Hostname?.GetHashCode() ?? 0);
            }
        }
    }
}