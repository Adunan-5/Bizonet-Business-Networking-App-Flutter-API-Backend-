namespace Bizonet.Api.Models.Dtos.Referrals;

public class SendReferralRequestDto
{
    public string ReferralType { get; set; } = ""; // self / outsider

    public int GroupId { get; set; }

    // recipient user (must exist)
    public string ReferredToEmail { get; set; } = "";

    public string? Remarks { get; set; }

    // outsider fields (required only for outsider referralType)
    public string? OutsiderName { get; set; }
    public string? OutsiderMobileNo { get; set; }
    public string? OutsiderEmail { get; set; }
    public string? OutsiderAddress { get; set; }
}
