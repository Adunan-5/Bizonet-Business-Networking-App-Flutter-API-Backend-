using Bizonet.Api.Models.Dtos.Business;
using Bizonet.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bizonet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusinessController : ControllerBase
    {
        private readonly IBusinessService _businessService;

        public BusinessController(IBusinessService businessService)
        {
            _businessService = businessService;
        }

        [Authorize]
        [HttpPost("save")]
        public async Task<IActionResult> SaveBusinessInfo(SaveBusinessInfoRequestDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return Unauthorized(new { error = "Invalid token" });

            var result = await _businessService.SaveBusinessInfoAsync(userId, dto);

            if (!result.ok)
                return BadRequest(new { error = result.message });

            return Ok(new { success = result.message, data = result.data });
        }
    }
}
