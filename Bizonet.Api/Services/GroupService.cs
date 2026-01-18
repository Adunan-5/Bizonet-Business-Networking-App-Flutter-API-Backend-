using Bizonet.Api.Data;
using Bizonet.Api.Models.Dtos.Groups;
using Bizonet.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bizonet.Api.Services
{
    public interface IGroupService
    {
        Task<(bool ok, string message, object? data)> CreateGroupAsync(int userId, CreateGroupRequestDto dto);
        Task<(bool ok, string message)> JoinGroupAsync(int userId, JoinGroupRequestDto dto);
        Task<(bool ok, string message, object? data)> GetPendingRequestsAsync(int ownerId, int groupId);
        Task<(bool ok, string message)> DecideMemberAsync(int ownerId, int groupId, MemberDecisionRequestDto dto);

    }

    public class GroupService : IGroupService
    {
        private readonly AppDbContext _db;

        public GroupService(AppDbContext db)
        {
            _db = db;
        }

        private async Task<string> GenerateUniqueGroupCodeAsync()
        {
            while (true)
            {
                var code = "BIZ" + Guid.NewGuid().ToString("N")[..6].ToUpper();

                var exists = await _db.Groups.AnyAsync(x => x.GroupCode == code);
                if (!exists)
                    return code;
            }
        }

        public async Task<(bool ok, string message, object? data)> CreateGroupAsync(int userId, CreateGroupRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.GroupName))
                return (false, "GroupName is required", null);

            // check duplicate group by same owner
            var existing = await _db.Groups
                .FirstOrDefaultAsync(x => x.OwnerId == userId && x.GroupName == dto.GroupName);

            if (existing != null && existing.Active != 3)
                return (false, "Group can only be resubmitted if it is in rejected state", null);

            // if rejected -> resubmit
            if (existing != null && existing.Active == 3)
            {
                existing.AboutGroup = dto.AboutGroup;
                existing.Tags = dto.Tags;
                existing.Active = 2;

                await _db.SaveChangesAsync();

                return (true, "Group resubmitted for approval", new
                {
                    existing.GroupId,
                    existing.GroupName,
                    existing.AboutGroup,
                    existing.Tags,
                    existing.GroupCode,
                    existing.Active
                });
            }

            // create new group
            var groupCode = await GenerateUniqueGroupCodeAsync();

            var group = new Group
            {
                GroupName = dto.GroupName,
                OwnerId = userId,
                AboutGroup = dto.AboutGroup,
                Tags = dto.Tags,
                GroupCode = groupCode,
                Active = 2,
                CreatedAt = DateTime.UtcNow
            };

            _db.Groups.Add(group);
            await _db.SaveChangesAsync();

            // Add owner to GroupUsers (Active = 1)
            int? businessCategoryId = await _db.BusinessInfos
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.BusinessInfoId)
                .Select(x => x.BusinessCategoryId)
                .FirstOrDefaultAsync();

            var groupUser = new GroupUser
            {
                UserId = userId,
                GroupId = group.GroupId,
                BusinessCategoryId = businessCategoryId,
                Active = 1,
                CreatedAt = DateTime.UtcNow
            };

            _db.GroupUsers.Add(groupUser);
            await _db.SaveChangesAsync();

            return (true, "Group created successfully", new
            {
                group.GroupId,
                group.GroupName,
                group.AboutGroup,
                group.Tags,
                group.GroupCode,
                group.Active
            });
        }

        public async Task<(bool ok, string message)> JoinGroupAsync(int userId, JoinGroupRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.GroupCode))
                return (false, "GroupCode is required");

            var group = await _db.Groups.FirstOrDefaultAsync(x => x.GroupCode == dto.GroupCode && x.Active == 1);

            if (group == null)
                return (false, "Group not found or not active");

            // determine businessCategoryId
            int? businessCategoryId = dto.BusinessCategoryId;

            if (businessCategoryId == null)
            {
                businessCategoryId = await _db.BusinessInfos
                    .Where(x => x.UserId == userId)
                    .OrderByDescending(x => x.BusinessInfoId)
                    .Select(x => x.BusinessCategoryId)
                    .FirstOrDefaultAsync();

                if (businessCategoryId == null)
                    return (false, "No business category found for this user. Please set business info first.");
            }

            // check duplicate join per category
            var exists = await _db.GroupUsers.AnyAsync(x =>
                x.UserId == userId &&
                x.GroupId == group.GroupId &&
                x.BusinessCategoryId == businessCategoryId);

            if (exists)
                return (false, "You already joined this group under this category.");

            // create membership pending approval (Active=2)
            var join = new GroupUser
            {
                UserId = userId,
                GroupId = group.GroupId,
                BusinessCategoryId = businessCategoryId,
                Active = 2,
                CreatedAt = DateTime.UtcNow
            };

            _db.GroupUsers.Add(join);
            await _db.SaveChangesAsync();

            // notifications will be handled later
            return (true, "Approval has been sent to the group admin for joining the group");
        }

        public async Task<(bool ok, string message, object? data)> GetPendingRequestsAsync(int ownerId, int groupId)
        {
            var group = await _db.Groups.FirstOrDefaultAsync(x => x.GroupId == groupId);
            if (group == null)
                return (false, "Group not found", null);

            if (group.OwnerId != ownerId)
                return (false, "Unauthorized. Only group owner can view requests.", null);

            var requests = await _db.GroupUsers
                .Where(x => x.GroupId == groupId && x.Active == 2)
                .Join(_db.Users,
                    gu => gu.UserId,
                    u => u.UserId,
                    (gu, u) => new
                    {
                        u.UserId,
                        u.FirstName,
                        u.LastName,
                        u.Email,
                        u.MobileNo,
                        gu.BusinessCategoryId,
                        gu.CreatedAt
                    })
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return (true, "Pending requests fetched successfully.", requests);
        }

        public async Task<(bool ok, string message)> DecideMemberAsync(int ownerId, int groupId, MemberDecisionRequestDto dto)
        {
            var action = dto.Action.Trim().ToLower();
            if (action != "approve" && action != "reject")
                return (false, "Invalid action. Use approve or reject");

            var group = await _db.Groups.FirstOrDefaultAsync(x => x.GroupId == groupId && x.Active == 1);
            if (group == null)
                return (false, "Group not found or not active");

            if (group.OwnerId != ownerId)
                return (false, "Unauthorized. Only group owner can approve/reject members");

            // locate membership
            var memberRow = await _db.GroupUsers.FirstOrDefaultAsync(x =>
                x.GroupId == groupId &&
                x.UserId == dto.UserId &&
                x.Active == 2 &&
                (dto.BusinessCategoryId == null || x.BusinessCategoryId == dto.BusinessCategoryId));

            if (memberRow == null)
                return (false, "Join request not found or already processed");

            if (action == "approve")
                memberRow.Active = 1;
            else
                memberRow.Active = 3;

            await _db.SaveChangesAsync();

            return (true, $"Member {action}d successfully");
        }

    }
}
