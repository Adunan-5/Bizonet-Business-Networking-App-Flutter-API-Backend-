using System.ComponentModel.DataAnnotations;

namespace Bizonet.Api.Models.Dtos.Auth
{
    public class RegisterRequestDto
    {
        [Required] public string FirstName { get; set; } = null!;
        [Required] public string LastName { get; set; } = null!;
        [Required, EmailAddress] public string Email { get; set; } = null!;
        [Required] public string MobileNo { get; set; } = null!;

        public string? CountryCode { get; set; }
        public string? CountryShortCode { get; set; }

        [Required] public string Password { get; set; } = null!;
        [Required] public string ConfirmPassword { get; set; } = null!;

        public string? Address { get; set; }
        public string? ZipCode { get; set; }

        // ✅ names -> ids
        [Required] public string CountryName { get; set; } = null!;
        [Required] public string StateName { get; set; } = null!;
        [Required] public string CityName { get; set; } = null!;
    }
}
