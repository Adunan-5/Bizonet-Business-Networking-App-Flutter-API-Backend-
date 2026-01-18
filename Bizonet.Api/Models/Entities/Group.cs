namespace Bizonet.Api.Models.Entities
{
    public class Group
    {
        public int GroupId { get; set; }

        public string? GroupName { get; set; }
        public int OwnerId { get; set; }

        public string GroupCode { get; set; } = null!;

        public string? AboutGroup { get; set; }
        public string? Tags { get; set; }

        // 0 inactive, 1 active, 2 pending, 3 rejected
        public byte Active { get; set; } = 2;
        public bool BizonetApproved { get; set; } = false;
        public string? GroupLink { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User Owner { get; set; } = null!;
        public ICollection<GroupUser> Members { get; set; } = new List<GroupUser>();
    }
}
