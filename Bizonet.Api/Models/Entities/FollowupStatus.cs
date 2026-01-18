namespace Bizonet.Api.Models.Entities;

public class FollowupStatus
{
    public int FollowupStatusId { get; set; }
    public string? FollowupStatusName { get; set; }

    public byte So { get; set; } = 0;
    public byte Active { get; set; } = 1;

    public ICollection<FollowupComment> Comments { get; set; } = new List<FollowupComment>();
}
