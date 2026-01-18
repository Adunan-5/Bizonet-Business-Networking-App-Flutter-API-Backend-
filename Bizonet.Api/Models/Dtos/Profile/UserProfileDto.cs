namespace Bizonet.Api.Models.Dtos.Profile
{
    public class UserProfileDto
    {
        public int UserId { get; set; }

        public string? Email { get; set; }
        public string? UserName { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? MobileNo { get; set; }
        public string? MobileCountryCode { get; set; }
        public string? MobileCountryShortCode { get; set; }

        public string? Address { get; set; }
        public string? Designation { get; set; }

        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }
        public string? ZipCode { get; set; }

        public string? CountryName { get; set; }
        public string? StateName { get; set; }
        public string? CityName { get; set; }

        public string ProfilePhoto { get; set; } = "";
        public string UserRole { get; set; } = "user";
    }
}
