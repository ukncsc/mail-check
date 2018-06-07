namespace Dmarc.Common.Api.Identity.Domain
{
    public class Identity
    {
        public Identity(int id, string firstName, string lastName, string email, string roleType)
        {
            Id = id;
            FirstName = firstName ?? string.Empty;
            LastName = lastName ?? string.Empty;
            Email = email;
            RoleType = roleType;
        }

        public int Id { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Email { get; }
        public string RoleType { get; }
    }
}
