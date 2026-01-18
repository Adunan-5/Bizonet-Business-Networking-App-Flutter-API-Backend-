using Bizonet.Api.Models.Dtos.Groups;
using Bizonet.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bizonet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _service;

        public GroupsController(IGroupService service)
        {
            _service = service;
        }

        private int? GetUserId()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userIdStr)) return null;
            if (!int.TryParse(userIdStr, out var userId)) return null;
            return userId;
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateGroupRequestDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized(new { error = "Invalid token" });

            var result = await _service.CreateGroupAsync(userId.Value, dto);

            if (!result.ok)
                return BadRequest(new { error = result.message });

            return Ok(new { success = result.message, data = result.data });
        }

        [Authorize]
        [HttpPost("join")]
        public async Task<IActionResult> Join([FromBody] JoinGroupRequestDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized(new { error = "Invalid token" });

            var result = await _service.JoinGroupAsync(userId.Value, dto);

            if (!result.ok)
                return BadRequest(new { error = result.message });

            return Ok(new { success = result.message });
        }

        [Authorize]
        [HttpGet("{groupId}/requests")]
        public async Task<IActionResult> GetRequests(int groupId)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized(new { error = "Invalid token" });

            var result = await _service.GetPendingRequestsAsync(userId.Value, groupId);

            if (!result.ok)
                return BadRequest(new { error = result.message });

            return Ok(new
            {
                success = result.message,
                requests = result.data
            });
        }

        [Authorize]
        [HttpPost("{groupId}/members/decision")]
        public async Task<IActionResult> DecideMember(int groupId, [FromBody] MemberDecisionRequestDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized(new { error = "Invalid token" });

            var result = await _service.DecideMemberAsync(userId.Value, groupId, dto);

            if (!result.ok)
                return BadRequest(new { error = result.message });

            return Ok(new { success = result.message });
        }

    }
}
