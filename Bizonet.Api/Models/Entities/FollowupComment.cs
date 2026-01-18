namespace Bizonet.Api.Models.Entities;

public class FollowupComment
{
    public int FollowupCommentsId { get; set; }

    public int? FollowupStatusId { get; set; }
    public FollowupStatus? FollowupStatus { get; set; }

    public string? FollowupCommentsName { get; set; }

    public byte So { get; set; } = 0;
    public byte Active { get; set; } = 1;
}
