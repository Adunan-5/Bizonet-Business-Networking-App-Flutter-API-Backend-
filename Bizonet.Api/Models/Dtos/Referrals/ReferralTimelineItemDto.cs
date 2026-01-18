namespace Bizonet.Api.Models.Dtos.Referrals;

public class ReferralTimelineItemDto
{
    public int ReferralStatusId { get; set; }

    public string? ReferralReceivedStatus { get; set; } // pending/Accept/Reject

    public string? RejectReason { get; set; }

    public int? FollowupStatusId { get; set; }
    public string? FollowupStatusName { get; set; }

    public int? FollowupCommentsId { get; set; }
    public string? FollowupCommentsName { get; set; }

    public DateTime? FollowupNextDate { get; set; }

    public DateTime DateCreated { get; set; }
}
