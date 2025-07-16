using BrochureAPI.DTOs;
using BrochureAPI.Models;

namespace BrochureAPI.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto?> RefreshTokenAsync(TokenDto tokenDto);
        string GenerateJwtToken(User user);
        string GenerateRefreshToken();
        bool ValidateToken(string token);
        string? GetUserIdFromToken(string token);
    }
} 