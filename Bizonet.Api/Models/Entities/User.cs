namespace Bizonet.Api.Models.Entities;

public class User
{
    public int UserId { get; set; }  // Identity PK

    public string? UserName { get; set; }

    public string PasswordHash { get; set; } = null!;

    public int? BusinessCategoryId { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? CountryCode { get; set; }
    public string? CountryShortCode { get; set; }

    public string? MobileNo { get; set; }

    public string? Address { get; set; }
    public string? Designation { get; set; }

    public int? CountryId { get; set; }
    public int? StateId { get; set; }
    public int? CityId { get; set; }

    public string? ZipCode { get; set; }

    public string ProfilePhoto { get; set; } = "defultprofilephoto.jpg";

    public bool Active { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool MobileVerified { get; set; } = false;
    public bool EmailVerified { get; set; } = false;

    public string UserRole { get; set; } = "user";

    // OTP navigation
    public ICollection<UserOtp> Otps { get; set; } = new List<UserOtp>();
}
