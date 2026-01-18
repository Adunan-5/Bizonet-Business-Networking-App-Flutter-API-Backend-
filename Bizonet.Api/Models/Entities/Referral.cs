namespace Bizonet.Api.Models.Entities;

public class Referral
{
    public int ReferralId { get; set; }

    public int? ReferredByUserId { get; set; }
    public User? ReferredByUser { get; set; }

    public int? ReferredToUserId { get; set; }
    public User? ReferredToUser { get; set; }

    public string? ReferralType { get; set; } // self / outsider

    public int? GroupId { get; set; }
    public Group? Group { get; set; }

    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    public ReferralOutsider? Outsider { get; set; }
    public ICollection<ReferralStatus> StatusHistory { get; set; } = new List<ReferralStatus>();
}
