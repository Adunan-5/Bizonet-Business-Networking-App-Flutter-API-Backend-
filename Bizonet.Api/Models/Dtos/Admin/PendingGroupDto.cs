namespace Bizonet.Api.Models.Dtos.Admin
{
    public class PendingGroupDto
    {
        public int GroupId { get; set; }
        public string? GroupName { get; set; }
        public int OwnerId { get; set; }
        public string? OwnerName { get; set; }
        public string? OwnerEmail { get; set; }
        public DateTime CreatedAt { get; set; }
        public byte Active { get; set; }
    }
}
