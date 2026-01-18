using System.ComponentModel.DataAnnotations;

namespace Bizonet.Api.Models.Dtos.Auth
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = null!;
    }
}
