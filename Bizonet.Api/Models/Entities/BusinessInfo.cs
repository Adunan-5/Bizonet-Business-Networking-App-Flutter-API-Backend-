namespace Bizonet.Api.Models.Entities
{
    public class BusinessInfo
    {
        public int BusinessInfoId { get; set; }

        public int UserId { get; set; }

        public int? BusinessCategoryId { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyAddress { get; set; }
        public string? BusinessDescription { get; set; }
        public string? CompanyWebsite { get; set; }

        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }
        public string? ZipCode { get; set; }

        public string? BusinessEmail { get; set; }
        public string? CountryCode { get; set; }
        public string? CountryShortCode { get; set; }
        public string? BusinessPhone { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
    }
}
