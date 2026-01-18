using Microsoft.AspNetCore.Http;

namespace Bizonet.Api.Models.Dtos.Profile
{
    public class UpdateProfileRequestDto
    {
        public string? UserName { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? MobileNo { get; set; }
        public string? Address { get; set; }
        public string? Designation { get; set; }

        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }

        public string? ZipCode { get; set; }

        public IFormFile? ProfilePhoto { get; set; } // ✅ optional upload
    }
}
