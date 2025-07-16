using BrochureAPI.DTOs;
using BrochureAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BrochureAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            var response = await _authService.LoginAsync(loginDto);
            if (response == null)
            {
                return Unauthorized("Invalid email or password");
            }

            // Set JWT token in HTTP-only cookie
            SetTokenCookie(response.Token, response.RefreshToken);

            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken()
        {
            // Get token from cookies
            var accessToken = Request.Cookies["access_token"];
            var refreshToken = Request.Cookies["refresh_token"];

            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized("Invalid token data");
            }

            var tokenDto = new TokenDto
            {
                Token = accessToken,
                RefreshToken = refreshToken
            };

            var response = await _authService.RefreshTokenAsync(tokenDto);
            if (response == null)
            {
                return Unauthorized("Invalid token");
            }

            // Set new JWT token in HTTP-only cookie
            SetTokenCookie(response.Token, response.RefreshToken);

            return Ok(response);
        }

        [HttpPost("logout")]
        public ActionResult Logout()
        {
            // Clear cookies
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");

            return NoContent();
        }

        private void SetTokenCookie(string token, string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Only send cookie over HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7) // Match refresh token expiry
            };

            Response.Cookies.Append("access_token", token, cookieOptions);
            Response.Cookies.Append("refresh_token", refreshToken, cookieOptions);
        }
    }
} 