using IvyScans.API.Models.DTO;
using IvyScans.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IvyScans.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var authResponse = await _authService.LoginAsync(loginDto.Email, loginDto.Password);

            if (!authResponse.Success)
                return Unauthorized(new { message = authResponse.Message });

            return Ok(new
            {
                token = authResponse.Token,
                refreshToken = authResponse.RefreshToken,
                user = authResponse.User
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            // Return the same structure as login endpoint
            return Ok(new
            {
                token = result.Token,
                refreshToken = result.RefreshToken,
                user = result.User
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            await _authService.LogoutAsync(userId);
            return Ok(new { message = "Logout successful" });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _authService.GetUserByIdAsync(userId);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            var result = await _authService.RefreshTokenAsync(refreshTokenDto.RefreshToken);

            if (!result.Success)
                return Unauthorized(new { message = result.Message });

            return Ok(new
            {
                token = result.Token,
                refreshToken = result.RefreshToken
            });
        }
    }
}