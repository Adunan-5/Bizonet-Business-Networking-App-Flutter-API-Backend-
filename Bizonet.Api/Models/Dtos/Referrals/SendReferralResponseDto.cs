namespace Bizonet.Api.Models.Dtos.Referrals;

public class SendReferralResponseDto
{
    public int ReferralId { get; set; }
    public string Status { get; set; } = "pending";
    public DateTime CreatedAt { get; set; }
}
