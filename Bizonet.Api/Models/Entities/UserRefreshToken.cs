namespace Bizonet.Api.Models.Entities
{
    public class UserRefreshToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }

        public bool IsRevoked { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
    }
}
