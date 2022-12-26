namespace Flitter.Api.Models.AuthDb
{
    public partial class User
    {
        public string Id { get; set; } = null!;
        public string? Email { get; set; }
        public string? EmailConstraint { get; set; }
        public bool EmailVerified { get; set; }
        public bool Enabled { get; set; }
        public string? FederationLink { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? RealmId { get; set; }
        public string? UserName { get; set; }
        public long? CreatedTimestamp { get; set; }
        public string? ServiceAccountClientLink { get; set; }
        public int NotBefore { get; set; }
    }
}
