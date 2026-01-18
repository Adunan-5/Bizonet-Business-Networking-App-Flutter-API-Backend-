namespace Bizonet.Api.Models.Dtos.Profile
{
    public class ProfileResponseDto
    {
        public UserProfileDto UserData { get; set; } = new();
        public BusinessInfoDto BusinessInformation { get; set; } = new();
    }
}
