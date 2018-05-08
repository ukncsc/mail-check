namespace Dmarc.Admin.Api.Domain
{
    public class User
    {
        public User(int? id, string firstName, string lastName, string email, string roleType)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            RoleType = roleType;
        }

        public int? Id { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Email { get; }
        public string RoleType { get; }

        protected bool Equals(User other)
        {
            return Id == other.Id && 
                string.Equals(FirstName, other.FirstName) && 
                string.Equals(LastName, other.LastName) && 
                string.Equals(Email, other.Email) && 
                string.Equals(RoleType, other.RoleType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((User) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ (FirstName?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (LastName?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Email?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (RoleType?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}