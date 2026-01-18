namespace Bizonet.Api.Models.Entities;

public class ReferralStatus
{
    public int ReferralStatusId { get; set; }

    public int? ReferralId { get; set; }
    public Referral? Referral { get; set; }

    public int? ReferredByUserId { get; set; }
    public int? ReferredToUserId { get; set; }
    public int? GroupId { get; set; }

    public string? ReferralReceivedStatus { get; set; }  // pending/accepted/rejected
    public string? RejectReason { get; set; }

    public int? FollowupStatusId { get; set; }
    public FollowupStatus? FollowupStatus { get; set; }

    public int? FollowupCommentsId { get; set; }
    public FollowupComment? FollowupComment { get; set; }

    public DateTime? FollowupNextDate { get; set; } // better than varchar
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
}
