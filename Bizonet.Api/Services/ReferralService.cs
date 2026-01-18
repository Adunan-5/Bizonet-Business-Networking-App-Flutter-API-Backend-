using Bizonet.Api.Data;
using Bizonet.Api.Models.Dtos.Referrals;
using Bizonet.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bizonet.Api.Services;

public interface IReferralService
{
    Task<(bool ok, string message, SendReferralResponseDto? data)> SendReferralAsync(int senderUserId, SendReferralRequestDto dto);
    Task<(bool ok, string message)> UpdateReferralStatusAsync(int loggedInUserId, int referralId, UpdateReferralStatusRequestDto dto);
    Task<(bool ok, string message)> AddFollowupAsync(int loggedInUserId, int referralId, UpdateReferralFollowupRequestDto dto);
    Task<(bool ok, string message, List<ReferralListItemDto>? data)> GetInboxAsync(int loggedInUserId);
    Task<(bool ok, string message, List<ReferralListItemDto>? data)> GetSentAsync(int loggedInUserId);
    Task<(bool ok, string message, object? data)> GetReferralTimelineAsync(int loggedInUserId, int referralId);
    Task<(bool ok, string message, List<object>? data)> GetFollowupStatusesAsync();
    Task<(bool ok, string message, List<object>? data)> GetFollowupCommentsAsync(int followupStatusId);

}

public class ReferralService : IReferralService
{
    private readonly AppDbContext _db;

    public ReferralService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(bool ok, string message, SendReferralResponseDto? data)> SendReferralAsync(int senderUserId, SendReferralRequestDto dto)
    {
        var referralType = dto.ReferralType.Trim().ToLower();

        if (referralType != "self" && referralType != "outsider")
            return (false, "Invalid referralType. Allowed values: self, outsider", null);

        // ✅ group exists + active
        var group = await _db.Groups.FirstOrDefaultAsync(x => x.GroupId == dto.GroupId && x.Active == 1);
        if (group == null)
            return (false, "Group not found or not active", null);

        // ✅ sender must be active member in group
        var senderMember = await _db.GroupUsers.FirstOrDefaultAsync(x =>
            x.GroupId == dto.GroupId &&
            x.UserId == senderUserId &&
            x.Active == 1);

        if (senderMember == null)
            return (false, "You are not an active member of this group", null);

        // ✅ receiver user exists
        var receiver = await _db.Users.FirstOrDefaultAsync(x =>
            x.Email == dto.ReferredToEmail &&
            x.Active);

        if (receiver == null)
            return (false, "Referred-to user not found", null);

        // ✅ receiver must be active member in same group
        var receiverMember = await _db.GroupUsers.FirstOrDefaultAsync(x =>
            x.GroupId == dto.GroupId &&
            x.UserId == receiver.UserId &&
            x.Active == 1);

        if (receiverMember == null)
            return (false, "Referred-to user is not an active member of this group", null);

        // ✅ outsider validations
        if (referralType == "outsider")
        {
            if (string.IsNullOrWhiteSpace(dto.OutsiderName))
                return (false, "OutsiderName is required", null);

            if (string.IsNullOrWhiteSpace(dto.OutsiderMobileNo))
                return (false, "OutsiderMobileNo is required", null);
        }

        // ✅ DB transaction (like your PHP beginTransaction)
        using var tx = await _db.Database.BeginTransactionAsync();

        try
        {
            var referral = new Referral
            {
                ReferredByUserId = senderUserId,
                ReferredToUserId = receiver.UserId,
                ReferralType = referralType,
                GroupId = dto.GroupId,
                DateCreated = DateTime.UtcNow
            };

            _db.Referrals.Add(referral);
            await _db.SaveChangesAsync();

            if (referralType == "outsider")
            {
                var outsider = new ReferralOutsider
                {
                    ReferralId = referral.ReferralId,
                    ReferredTo = receiver.UserId,
                    OutsiderName = dto.OutsiderName,
                    OutsiderMobileNo = dto.OutsiderMobileNo,
                    OutsiderEmail = dto.OutsiderEmail,
                    OutsiderAddress = dto.OutsiderAddress,
                    Remarks = dto.Remarks,
                    DateCreated = DateTime.UtcNow
                };

                _db.ReferralOutsiders.Add(outsider);
                await _db.SaveChangesAsync();
            }

            // ✅ create first status row
            var status = new ReferralStatus
            {
                ReferralId = referral.ReferralId,
                ReferredByUserId = senderUserId,
                ReferredToUserId = receiver.UserId,
                GroupId = dto.GroupId,

                ReferralReceivedStatus = "pending",
                RejectReason = null,

                FollowupStatusId = null,
                FollowupCommentsId = null,
                FollowupNextDate = null,

                DateCreated = DateTime.UtcNow
            };

            _db.ReferralStatuses.Add(status);
            await _db.SaveChangesAsync();

            await tx.CommitAsync();

            return (true, "Referral sent successfully", new SendReferralResponseDto
            {
                ReferralId = referral.ReferralId,
                Status = "pending",
                CreatedAt = referral.DateCreated
            });
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }


    public async Task<(bool ok, string message)> UpdateReferralStatusAsync(int loggedInUserId, int referralId, UpdateReferralStatusRequestDto dto)
    {
        var status = dto.Status.Trim();

        if (status != "Accept" && status != "Reject")
            return (false, "Invalid status. Allowed values: Accept, Reject");

        var referral = await _db.Referrals.FirstOrDefaultAsync(x => x.ReferralId == referralId);
        if (referral == null)
            return (false, "Referral not found");

        // ✅ only recipient can update
        if (referral.ReferredToUserId != loggedInUserId)
            return (false, "Unauthorized. Only referred user can accept/reject.");

        // If Reject, reason required
        if (status == "Reject" && string.IsNullOrWhiteSpace(dto.RejectReason))
            return (false, "RejectReason is required");

        // ✅ Insert status row (history)
        var row = new ReferralStatus
        {
            ReferralId = referral.ReferralId,
            ReferredByUserId = referral.ReferredByUserId,
            ReferredToUserId = referral.ReferredToUserId,
            GroupId = referral.GroupId,

            ReferralReceivedStatus = status,
            RejectReason = status == "Reject" ? dto.RejectReason : null,

            FollowupStatusId = null,
            FollowupCommentsId = null,
            FollowupNextDate = null,

            DateCreated = DateTime.UtcNow
        };

        _db.ReferralStatuses.Add(row);
        await _db.SaveChangesAsync();

        return (true, "Referral status updated successfully");
    }

    public async Task<(bool ok, string message)> AddFollowupAsync(int loggedInUserId, int referralId, UpdateReferralFollowupRequestDto dto)
    {
        var referral = await _db.Referrals.FirstOrDefaultAsync(x => x.ReferralId == referralId);
        if (referral == null)
            return (false, "Referral not found");

        // ✅ only recipient can followup
        if (referral.ReferredToUserId != loggedInUserId)
            return (false, "Unauthorized. Only referred user can update followups.");

        // ✅ must be accepted first
        var lastStatus = await _db.ReferralStatuses
            .Where(x => x.ReferralId == referralId)
            .OrderByDescending(x => x.DateCreated)
            .FirstOrDefaultAsync();

        if (lastStatus == null)
            return (false, "Referral status history not found");

        if (lastStatus.ReferralReceivedStatus != "Accept")
            return (false, "Followup can be added only after referral is accepted");

        var statusName = dto.FollowupStatusName.Trim().ToLower();
        var commentName = dto.FollowupCommentsName.Trim();

        var followupStatus = await _db.FollowupStatuses
            .FirstOrDefaultAsync(x => x.FollowupStatusName!.ToLower() == statusName && x.Active == 1);

        if (followupStatus == null)
            return (false, "Follow-up status not found");

        var followupComment = await _db.FollowupComments
            .FirstOrDefaultAsync(x => x.FollowupCommentsName == commentName && x.Active == 1);

        if (followupComment == null)
            return (false, "Follow-up comment not found");

        // ✅ won/lost rule
        if (statusName != "won" && statusName != "lost")
        {
            if (dto.FollowupNextDate == null)
                return (false, "FollowupNextDate is required for this status");
        }
        else
        {
            if (dto.FollowupNextDate != null)
                return (false, "FollowupNextDate must be empty for 'won' or 'lost'");
        }

        // ✅ insert new row in ReferralStatuses as followup entry
        var row = new ReferralStatus
        {
            ReferralId = referralId,
            ReferredByUserId = referral.ReferredByUserId,
            ReferredToUserId = referral.ReferredToUserId,
            GroupId = referral.GroupId,

            ReferralReceivedStatus = "Accept", // keep accepted
            RejectReason = null,

            FollowupStatusId = followupStatus.FollowupStatusId,
            FollowupCommentsId = followupComment.FollowupCommentsId,
            FollowupNextDate = dto.FollowupNextDate,

            DateCreated = DateTime.UtcNow
        };

        _db.ReferralStatuses.Add(row);
        await _db.SaveChangesAsync();

        return (true, "Follow-up status updated successfully");
    }

    public async Task<(bool ok, string message, List<ReferralListItemDto>? data)> GetInboxAsync(int loggedInUserId)
    {
        var referrals = await _db.Referrals
            .Where(x => x.ReferredToUserId == loggedInUserId)
            .OrderByDescending(x => x.DateCreated)
            .Select(x => new ReferralListItemDto
            {
                ReferralId = x.ReferralId,
                ReferralType = x.ReferralType,
                GroupId = x.GroupId,
                GroupName = _db.Groups.Where(g => g.GroupId == x.GroupId).Select(g => g.GroupName).FirstOrDefault(),

                ReferredByUserId = x.ReferredByUserId,
                ReferredByEmail = _db.Users.Where(u => u.UserId == x.ReferredByUserId).Select(u => u.Email).FirstOrDefault(),
                ReferredByName =
                    (_db.Users.Where(u => u.UserId == x.ReferredByUserId).Select(u => u.FirstName).FirstOrDefault() ?? "") + " " +
                    (_db.Users.Where(u => u.UserId == x.ReferredByUserId).Select(u => u.LastName).FirstOrDefault() ?? ""),

                ReferredToUserId = x.ReferredToUserId,
                ReferredToEmail = _db.Users.Where(u => u.UserId == x.ReferredToUserId).Select(u => u.Email).FirstOrDefault(),
                ReferredToName =
                    (_db.Users.Where(u => u.UserId == x.ReferredToUserId).Select(u => u.FirstName).FirstOrDefault() ?? "") + " " +
                    (_db.Users.Where(u => u.UserId == x.ReferredToUserId).Select(u => u.LastName).FirstOrDefault() ?? ""),

                // ✅ outsider details only when referralType = outsider
                Outsider = x.ReferralType == "outsider"
                    ? _db.ReferralOutsiders
                        .Where(o => o.ReferralId == x.ReferralId)
                        .Select(o => new ReferralOutsiderDto
                        {
                            OutsiderName = o.OutsiderName,
                            OutsiderMobileNo = o.OutsiderMobileNo,
                            OutsiderEmail = o.OutsiderEmail,
                            OutsiderAddress = o.OutsiderAddress,
                            Remarks = o.Remarks
                        })
                        .FirstOrDefault()
                    : null,

                LatestStatus = _db.ReferralStatuses
                    .Where(s => s.ReferralId == x.ReferralId)
                    .OrderByDescending(s => s.DateCreated)
                    .Select(s => s.ReferralReceivedStatus)
                    .FirstOrDefault() ?? "pending",

                CreatedAt = x.DateCreated
            })
            .ToListAsync();

        return (true, "Inbox referrals fetched successfully", referrals);
    }

    public async Task<(bool ok, string message, List<ReferralListItemDto>? data)> GetSentAsync(int loggedInUserId)
    {
        var referrals = await _db.Referrals
            .Where(x => x.ReferredByUserId == loggedInUserId)
            .OrderByDescending(x => x.DateCreated)
            .Select(x => new ReferralListItemDto
            {
                ReferralId = x.ReferralId,
                ReferralType = x.ReferralType,
                GroupId = x.GroupId,
                GroupName = _db.Groups.Where(g => g.GroupId == x.GroupId).Select(g => g.GroupName).FirstOrDefault(),

                ReferredByUserId = x.ReferredByUserId,
                ReferredByEmail = _db.Users.Where(u => u.UserId == x.ReferredByUserId).Select(u => u.Email).FirstOrDefault(),
                ReferredByName =
                    (_db.Users.Where(u => u.UserId == x.ReferredByUserId).Select(u => u.FirstName).FirstOrDefault() ?? "") + " " +
                    (_db.Users.Where(u => u.UserId == x.ReferredByUserId).Select(u => u.LastName).FirstOrDefault() ?? ""),

                ReferredToUserId = x.ReferredToUserId,
                ReferredToEmail = _db.Users.Where(u => u.UserId == x.ReferredToUserId).Select(u => u.Email).FirstOrDefault(),
                ReferredToName =
                    (_db.Users.Where(u => u.UserId == x.ReferredToUserId).Select(u => u.FirstName).FirstOrDefault() ?? "") + " " +
                    (_db.Users.Where(u => u.UserId == x.ReferredToUserId).Select(u => u.LastName).FirstOrDefault() ?? ""),

                // ✅ outsider details only when referralType = outsider
                Outsider = x.ReferralType == "outsider"
                    ? _db.ReferralOutsiders
                        .Where(o => o.ReferralId == x.ReferralId)
                        .Select(o => new ReferralOutsiderDto
                        {
                            OutsiderName = o.OutsiderName,
                            OutsiderMobileNo = o.OutsiderMobileNo,
                            OutsiderEmail = o.OutsiderEmail,
                            OutsiderAddress = o.OutsiderAddress,
                            Remarks = o.Remarks
                        })
                        .FirstOrDefault()
                    : null,

                LatestStatus = _db.ReferralStatuses
                    .Where(s => s.ReferralId == x.ReferralId)
                    .OrderByDescending(s => s.DateCreated)
                    .Select(s => s.ReferralReceivedStatus)
                    .FirstOrDefault() ?? "pending",

                CreatedAt = x.DateCreated
            })
            .ToListAsync();

        return (true, "Sent referrals fetched successfully", referrals);
    }

    public async Task<(bool ok, string message, object? data)> GetReferralTimelineAsync(int loggedInUserId, int referralId)
    {
        var referral = await _db.Referrals.FirstOrDefaultAsync(x => x.ReferralId == referralId);
        if (referral == null)
            return (false, "Referral not found", null);

        // ✅ only sender OR receiver can view timeline
        if (referral.ReferredByUserId != loggedInUserId && referral.ReferredToUserId != loggedInUserId)
            return (false, "Unauthorized to view referral timeline", null);

        var timeline = await _db.ReferralStatuses
            .Where(x => x.ReferralId == referralId)
            .OrderBy(x => x.DateCreated)
            .Select(x => new ReferralTimelineItemDto
            {
                ReferralStatusId = x.ReferralStatusId,
                ReferralReceivedStatus = x.ReferralReceivedStatus,
                RejectReason = x.RejectReason,

                FollowupStatusId = x.FollowupStatusId,
                FollowupStatusName = x.FollowupStatus != null ? x.FollowupStatus.FollowupStatusName : null,

                FollowupCommentsId = x.FollowupCommentsId,
                FollowupCommentsName = x.FollowupComment != null ? x.FollowupComment.FollowupCommentsName : null,

                FollowupNextDate = x.FollowupNextDate,
                DateCreated = x.DateCreated
            })
            .ToListAsync();

        return (true, "Referral timeline fetched successfully", new
        {
            referralId = referral.ReferralId,
            referralType = referral.ReferralType,
            groupId = referral.GroupId,
            referredByUserId = referral.ReferredByUserId,
            referredToUserId = referral.ReferredToUserId,
            timeline
        });
    }

    public async Task<(bool ok, string message, List<object>? data)> GetFollowupStatusesAsync()
    {
        var statuses = await _db.FollowupStatuses
            .Where(x => x.Active == 1)
            .OrderBy(x => x.So)
            .Select(x => new
            {
                x.FollowupStatusId,
                x.FollowupStatusName
            })
            .ToListAsync();

        return (true, "Followup statuses fetched successfully", statuses.Cast<object>().ToList());
    }

    public async Task<(bool ok, string message, List<object>? data)> GetFollowupCommentsAsync(int followupStatusId)
    {
        var comments = await _db.FollowupComments
            .Where(x => x.Active == 1 && x.FollowupStatusId == followupStatusId)
            .OrderBy(x => x.So)
            .Select(x => new
            {
                x.FollowupCommentsId,
                x.FollowupStatusId,
                x.FollowupCommentsName
            })
            .ToListAsync();

        return (true, "Followup comments fetched successfully", comments.Cast<object>().ToList());
    }


}
