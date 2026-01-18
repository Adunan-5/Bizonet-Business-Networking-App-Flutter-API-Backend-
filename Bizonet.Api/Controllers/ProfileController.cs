using Bizonet.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bizonet.Api.Models.Dtos.Profile;
using System.Security.Claims;

namespace Bizonet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return Unauthorized(new { error = "Invalid token" });

            var result = await _profileService.GetMyProfileAsync(userId);

            if (!result.ok)
                return NotFound(new { error = result.message });

            return Ok(new
            {
                success = result.message,
                userData = result.data!.UserData,
                businessInformation = result.data.BusinessInformation
            });
        }

        [Authorize]
        [HttpPost("update")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileRequestDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return Unauthorized(new { error = "Invalid token" });

            var result = await _profileService.UpdateProfileAsync(userId, dto);

            if (!result.ok)
                return BadRequest(new { error = result.message });

            return Ok(new
            {
                success = result.message,
                updatedData = result.data
            });
        }


    }
}
