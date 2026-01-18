namespace Bizonet.Api.Models.Dtos.Groups
{
    public class MemberDecisionRequestDto
    {
        public int UserId { get; set; }
        public int? BusinessCategoryId { get; set; } // important for category-based joining
        public string Action { get; set; } = ""; // approve / reject
    }
}
