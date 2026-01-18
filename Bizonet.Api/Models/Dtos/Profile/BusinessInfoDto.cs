namespace Bizonet.Api.Models.Dtos.Profile
{
    public class BusinessInfoDto
    {
        public int? BusinessCategoryId { get; set; }
        public string? BusinessCategory { get; set; }

        public string? BusinessName { get; set; }
        public string? CompanyAddress { get; set; }
        public string? BusinessDescription { get; set; }
        public string? CompanyWebsite { get; set; }

        public string? BusinessEmail { get; set; }

        public string? BusinessPhone { get; set; }
        public string? MobileCountryCode { get; set; }
        public string? MobileCountryShortCode { get; set; }

        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }
        public string? ZipCode { get; set; }

        public string? CountryName { get; set; }
        public string? StateName { get; set; }
        public string? CityName { get; set; }
    }
}
