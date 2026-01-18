namespace Bizonet.Api.Models.Dtos.Referrals;

public class UpdateReferralFollowupRequestDto
{
    public string FollowupStatusName { get; set; } = "";
    public string FollowupCommentsName { get; set; } = "";
    public DateTime? FollowupNextDate { get; set; }
}
