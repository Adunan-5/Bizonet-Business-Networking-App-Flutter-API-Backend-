using System.ComponentModel.DataAnnotations;

namespace Bizonet.Api.Models.Dtos.Auth
{
    public class LogoutRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = null!;
    }
}
