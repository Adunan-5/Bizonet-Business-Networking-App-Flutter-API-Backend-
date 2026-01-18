using Bizonet.Api.Models.Dtos.Admin;
using Bizonet.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bizonet.Api.Controllers
{
    [ApiController]
    [Route("api/admin/groups")]
    [Authorize] // must be logged in
    public class AdminGroupsController : ControllerBase
    {
        private readonly IAdminGroupService _service;

        public AdminGroupsController(IAdminGroupService service)
        {
            _service = service;
        }

        [HttpPost("decision")]
        [Authorize(Roles = "admin")] // ✅ ONLY ADMIN CAN USE THIS
        public async Task<IActionResult> Decide([FromBody] GroupDecisionRequestDto dto)
        {
            var result = await _service.DecideGroupAsync(dto);

            if (!result.ok)
                return BadRequest(new { error = result.message });

            return Ok(new { success = result.message });
        }

        [HttpGet("pending")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Pending()
        {
            var result = await _service.GetPendingGroupsAsync();

            if (!result.ok)
                return BadRequest(new { error = result.message });

            return Ok(new
            {
                success = result.message,
                total = result.data.Count,
                data = result.data
            });
        }

    }
}
