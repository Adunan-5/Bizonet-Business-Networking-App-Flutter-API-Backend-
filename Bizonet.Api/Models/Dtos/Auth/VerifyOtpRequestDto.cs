using System.ComponentModel.DataAnnotations;

namespace Bizonet.Api.Models.Dtos.Auth
{
    public class VerifyOtpRequestDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Otp { get; set; } = null!;
    }
}
