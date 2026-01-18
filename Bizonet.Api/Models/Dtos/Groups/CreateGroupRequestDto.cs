namespace Bizonet.Api.Models.Dtos.Groups
{
    public class CreateGroupRequestDto
    {
        public string GroupName { get; set; } = "";
        public string AboutGroup { get; set; } = "";
        public string Tags { get; set; } = "";
    }
}
