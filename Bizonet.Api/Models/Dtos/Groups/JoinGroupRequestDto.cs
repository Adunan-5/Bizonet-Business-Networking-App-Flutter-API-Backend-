namespace Bizonet.Api.Models.Dtos.Groups
{
    public class JoinGroupRequestDto
    {
        public string GroupCode { get; set; } = "";
        public int? BusinessCategoryId { get; set; }
    }
}
