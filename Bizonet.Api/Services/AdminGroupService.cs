using Bizonet.Api.Data;
using Bizonet.Api.Models.Dtos.Admin;
using Microsoft.EntityFrameworkCore;

namespace Bizonet.Api.Services
{
    public interface IAdminGroupService
    {
        Task<(bool ok, string message)> DecideGroupAsync(GroupDecisionRequestDto dto);
        Task<(bool ok, string message, List<PendingGroupDto> data)> GetPendingGroupsAsync();
    }

    public class AdminGroupService : IAdminGroupService
    {
        private readonly AppDbContext _db;

        public AdminGroupService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<(bool ok, string message)> DecideGroupAsync(GroupDecisionRequestDto dto)
        {
            if (dto.GroupId <= 0)
                return (false, "GroupId is required");

            var action = dto.Action.Trim().ToLower();

            if (action != "approve" && action != "reject")
                return (false, "Invalid action. Use approve or reject");

            var group = await _db.Groups.FirstOrDefaultAsync(x => x.GroupId == dto.GroupId && x.Active == 2);

            if (group == null)
                return (false, "Group not found or already processed");

            if (action == "approve")
            {
                group.Active = 1;
                group.BizonetApproved = true;

                // create unique link if missing
                if (string.IsNullOrWhiteSpace(group.GroupLink))
                {
                    var slug = (group.GroupName ?? "group")
                        .Trim()
                        .ToLower()
                        .Replace(" ", "-");

                    group.GroupLink = $"{slug}-{group.GroupId}";
                }

                await _db.SaveChangesAsync();
                return (true, "Group approved successfully");
            }

            // reject
            group.Active = 3;
            await _db.SaveChangesAsync();
            return (true, "Group rejected successfully");
        }

        public async Task<(bool ok, string message, List<PendingGroupDto> data)> GetPendingGroupsAsync()
        {
            var pending = await _db.Groups
                .Where(x => x.Active == 2)
                .Join(_db.Users,
                      g => g.OwnerId,
                      u => u.UserId,
                      (g, u) => new PendingGroupDto
                      {
                          GroupId = g.GroupId,
                          GroupName = g.GroupName,
                          OwnerId = g.OwnerId,
                          OwnerName = (u.FirstName ?? "") + " " + (u.LastName ?? ""),
                          OwnerEmail = u.Email,
                          CreatedAt = g.CreatedAt,
                          Active = g.Active
                      })
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return (true, "Pending groups fetched successfully", pending);
        }

    }
}
