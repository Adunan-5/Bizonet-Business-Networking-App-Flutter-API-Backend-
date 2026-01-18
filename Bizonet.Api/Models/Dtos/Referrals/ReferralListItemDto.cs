namespace Bizonet.Api.Models.Dtos.Referrals;

public class ReferralListItemDto
{
    public int ReferralId { get; set; }

    public string? ReferralType { get; set; } // self / outsider
    public int? GroupId { get; set; }
    public string? GroupName { get; set; }

    public int? ReferredByUserId { get; set; }
    public string? ReferredByName { get; set; }
    public string? ReferredByEmail { get; set; }

    public int? ReferredToUserId { get; set; }
    public string? ReferredToName { get; set; }
    public string? ReferredToEmail { get; set; }
    public ReferralOutsiderDto? Outsider { get; set; }

    public string? LatestStatus { get; set; } // pending / Accept / Reject

    public DateTime CreatedAt { get; set; }
}
