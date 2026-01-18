using Bizonet.Api.Models.Dtos.Auth;
using Bizonet.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bizonet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto dto)
        {
            var result = await _authService.RegisterAsync(dto);

            if (!result.ok)
                return BadRequest(new { error = result.message });

            return Ok(new { success = result.message });
        }

        [HttpPut("verify-otp")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpRequestDto dto)
        {
            var result = await _authService.VerifyOtpAsync(dto);

            if (!result.ok)
                return BadRequest(new { error = result.message });

            return Ok(new { success = result.message });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto dto)
        {
            var result = await _authService.LoginAsync(dto);

            if (!result.ok)
                return BadRequest(new { error = result.message });

            // OTP send method returns only success message
            if (result.data == null)
                return Ok(new { success = result.message });

            return Ok(new
            {
                success = result.message,
                token = result.data.AccessToken,
                refreshToken = result.data.RefreshToken,
                expiresAt = result.data.ExpiresAt,
                userData = result.data.UserData
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto dto)
        {
            var result = await _authService.RefreshTokenAsync(dto);

            if (!result.ok)
                return BadRequest(new { error = result.message });

            return Ok(new
            {
                success = result.message,
                token = result.data!.AccessToken,
                refreshToken = result.data.RefreshToken,
                expiresAt = result.data.ExpiresAt,
                userData = result.data.UserData
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(LogoutRequestDto dto)
        {
            var result = await _authService.LogoutAsync(dto);

            if (!result.ok)
                return BadRequest(new { error = result.message });

            return Ok(new { success = result.message });
        }

    }
}
