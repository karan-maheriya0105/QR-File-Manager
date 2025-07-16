using BrochureAPI.DTOs;
using BrochureAPI.Models;

namespace BrochureAPI.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(string id);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserDto?> UpdateUserAsync(string id, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(string id);
        Task<UserDto?> AuthenticateAsync(LoginDto loginDto);
        Task<string> HashPasswordAsync(string password);
        Task<bool> VerifyPasswordAsync(string password, string hashedPassword);
    }
} 