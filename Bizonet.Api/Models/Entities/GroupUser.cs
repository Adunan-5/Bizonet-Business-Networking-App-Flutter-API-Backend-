namespace Bizonet.Api.Models.Entities
{
    public class GroupUser
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int GroupId { get; set; }

        public int? BusinessCategoryId { get; set; }

        // 0 inactive, 1 active, 2 pending, 3 abusive/rejected
        public byte Active { get; set; } = 2;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
        public Group Group { get; set; } = null!;
        public BusinessCategory? BusinessCategory { get; set; }
    }
}
