namespace Bizonet.Api.Models.Entities;

public class ReferralOutsider
{
    public int OutsiderId { get; set; }

    public int? ReferralId { get; set; }
    public Referral? Referral { get; set; }

    public int? ReferredTo { get; set; } // referredToUserId

    public string? OutsiderName { get; set; }
    public string? OutsiderMobileNo { get; set; }
    public string? OutsiderEmail { get; set; }
    public string? OutsiderAddress { get; set; }

    public string? Remarks { get; set; }

    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
}
