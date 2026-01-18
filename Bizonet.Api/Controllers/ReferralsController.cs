using Bizonet.Api.Models.Dtos.Referrals;
using Bizonet.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bizonet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReferralsController : ControllerBase
{
    private readonly IReferralService _service;

    public ReferralsController(IReferralService service)
    {
        _service = service;
    }

    [Authorize]
    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] SendReferralRequestDto dto)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
            return Unauthorized(new { error = "Invalid token" });

        var result = await _service.SendReferralAsync(userId, dto);

        if (!result.ok)
            return BadRequest(new { error = result.message });

        return Ok(new
        {
            success = result.message,
            data = result.data
        });
    }

    [Authorize]
    [HttpPost("{referralId}/status")]
    public async Task<IActionResult> UpdateStatus(int referralId, [FromBody] UpdateReferralStatusRequestDto dto)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
            return Unauthorized(new { error = "Invalid token" });

        var result = await _service.UpdateReferralStatusAsync(userId, referralId, dto);

        if (!result.ok)
            return BadRequest(new { error = result.message });

        return Ok(new { success = result.message });
    }

    [Authorize]
    [HttpPost("{referralId}/followup")]
    public async Task<IActionResult> Followup(int referralId, [FromBody] UpdateReferralFollowupRequestDto dto)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
            return Unauthorized(new { error = "Invalid token" });

        var result = await _service.AddFollowupAsync(userId, referralId, dto);

        if (!result.ok)
            return BadRequest(new { error = result.message });

        return Ok(new { success = result.message });
    }

    private int? GetUserId()
    {
        var claim = User.Claims.FirstOrDefault(x => x.Type.EndsWith("nameidentifier"));
        if (claim == null) return null;

        return int.TryParse(claim.Value, out var id) ? id : null;
    }

    // ✅ Inbox referrals (received)
    [HttpGet("inbox")]
    public async Task<IActionResult> Inbox()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized(new { error = "Invalid token" });

        var result = await _service.GetInboxAsync(userId.Value);

        if (!result.ok)
            return BadRequest(new { error = result.message });

        return Ok(new
        {
            success = result.message,
            referrals = result.data
        });
    }

    // ✅ Sent referrals
    [HttpGet("sent")]
    public async Task<IActionResult> Sent()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized(new { error = "Invalid token" });

        var result = await _service.GetSentAsync(userId.Value);

        if (!result.ok)
            return BadRequest(new { error = result.message });

        return Ok(new
        {
            success = result.message,
            referrals = result.data
        });
    }

    // ✅ Referral timeline
    [HttpGet("{referralId:int}/timeline")]
    public async Task<IActionResult> Timeline(int referralId)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized(new { error = "Invalid token" });

        var result = await _service.GetReferralTimelineAsync(userId.Value, referralId);

        if (!result.ok)
            return BadRequest(new { error = result.message });

        return Ok(new
        {
            success = result.message,
            data = result.data
        });
    }

    [Authorize]
    [HttpGet("followup-statuses")]
    public async Task<IActionResult> GetFollowupStatuses()
    {
        var result = await _service.GetFollowupStatusesAsync();

        if (!result.ok)
            return BadRequest(new { error = result.message });

        return Ok(new
        {
            success = result.message,
            data = result.data
        });
    }

    [Authorize]
    [HttpGet("followup-comments/{followupStatusId:int}")]
    public async Task<IActionResult> GetFollowupComments(int followupStatusId)
    {
        var result = await _service.GetFollowupCommentsAsync(followupStatusId);

        if (!result.ok)
            return BadRequest(new { error = result.message });

        return Ok(new
        {
            success = result.message,
            data = result.data
        });
    }

}
