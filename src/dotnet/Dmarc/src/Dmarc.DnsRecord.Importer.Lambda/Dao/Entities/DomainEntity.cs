namespace Dmarc.DnsRecord.Importer.Lambda.Dao.Entities
{
    public class DomainEntity
    {
        public DomainEntity(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; }
        public string Name { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DomainEntity) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id * 397) ^ (Name?.GetHashCode() ?? 0);
            }
        }

        protected bool Equals(DomainEntity other)
        {
            return Id == other.Id && string.Equals(Name, other.Name);
        }
    }
}