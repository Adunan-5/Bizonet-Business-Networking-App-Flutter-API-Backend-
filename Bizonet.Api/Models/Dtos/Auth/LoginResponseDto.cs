namespace Bizonet.Api.Models.Dtos.Auth
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }

        public object UserData { get; set; } = null!;
    }
}
