namespace Bizonet.Api.Models.Dtos.Business
{
    public class SaveBusinessInfoRequestDto
    {
        public string? CategoryName { get; set; }

        public string? CompanyName { get; set; }
        public string? CompanyAddress { get; set; }
        public string? BusinessDescription { get; set; }
        public string? CompanyWebsite { get; set; }

        // ✅ Company contact email (NOT user email)
        public string? BusinessEmail { get; set; }

        public string? BusinessPhone { get; set; }
        public string? CountryCode { get; set; }
        public string? CountryShortCode { get; set; }

        // names -> ids
        public string? CountryName { get; set; }
        public string? StateName { get; set; }
        public string? CityName { get; set; }

        public string? ZipCode { get; set; }
    }
}
