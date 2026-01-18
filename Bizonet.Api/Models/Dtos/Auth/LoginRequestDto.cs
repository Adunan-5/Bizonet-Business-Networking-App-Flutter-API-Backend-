using System.ComponentModel.DataAnnotations;

namespace Bizonet.Api.Models.Dtos.Auth
{
    public class LoginRequestDto
    {
        [Required]
        public string UserInput { get; set; } = null!; // email or mobile

        [Required]
        public string SignInMethod { get; set; } = null!; // password | otp | otpValidation

        public string? Password { get; set; } // used only in password login
        public string? EnteredOtp { get; set; } // used only in otpValidation
    }
}
