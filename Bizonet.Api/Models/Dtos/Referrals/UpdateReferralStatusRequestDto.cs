namespace Bizonet.Api.Models.Dtos.Referrals;

public class UpdateReferralStatusRequestDto
{
    public string Status { get; set; } = ""; // Accept / Reject
    public string? RejectReason { get; set; }
}
