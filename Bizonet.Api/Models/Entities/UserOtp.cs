namespace Bizonet.Api.Models.Entities;

public class UserOtp
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string OtpCode { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }

    public bool IsUsed { get; set; } = false;

    public string Purpose { get; set; } = "RegisterEmailOtp";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}
