namespace Bizonet.Api.Models.Dtos.Admin
{
    public class GroupDecisionRequestDto
    {
        public int GroupId { get; set; }
        public string Action { get; set; } = ""; // approve / reject
    }
}
