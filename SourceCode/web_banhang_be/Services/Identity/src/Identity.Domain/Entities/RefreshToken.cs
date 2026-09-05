

namespace AuraMart.Identity.Domain
{
    public class RefreshToken:Entity
    {
        public Guid UserID { get; set; }
        public virtual User User { get; set; }= default!;
        public string TokenHash { get; set; }= default!;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime? RevokedAt { get; set; }
    }
}

