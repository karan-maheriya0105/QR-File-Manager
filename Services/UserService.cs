using BrochureAPI.Data;
using BrochureAPI.DTOs;
using BrochureAPI.Interfaces;
using BrochureAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace BrochureAPI.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            return users.Select(MapToDto);
        }

        public async Task<UserDto?> GetUserByIdAsync(string id)
        {
            var user = await _context.Users.FindAsync(id);
            return user != null ? MapToDto(user) : null;
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.StrEmailId == email);
            return user != null ? MapToDto(user) : null;
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            var hashedPassword = await HashPasswordAsync(createUserDto.StrPassword);

            var user = new User
            {
                StrName = createUserDto.StrName,
                StrEmailId = createUserDto.StrEmailId,
                StrPassword = hashedPassword,
                BolIsAdmin = createUserDto.BolIsAdmin
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return MapToDto(user);
        }

        public async Task<UserDto?> UpdateUserAsync(string id, UpdateUserDto updateUserDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return null;

            if (!string.IsNullOrEmpty(updateUserDto.StrName))
                user.StrName = updateUserDto.StrName;

            if (!string.IsNullOrEmpty(updateUserDto.StrEmailId))
                user.StrEmailId = updateUserDto.StrEmailId;

            if (!string.IsNullOrEmpty(updateUserDto.StrPassword))
                user.StrPassword = await HashPasswordAsync(updateUserDto.StrPassword);

            if (updateUserDto.BolIsAdmin.HasValue)
                user.BolIsAdmin = updateUserDto.BolIsAdmin.Value;

            await _context.SaveChangesAsync();
            return MapToDto(user);
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UserDto?> AuthenticateAsync(LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.StrEmailId == loginDto.StrEmailId);
            if (user == null)
                return null;

            var isPasswordValid = await VerifyPasswordAsync(loginDto.StrPassword, user.StrPassword);
            return isPasswordValid ? MapToDto(user) : null;
        }

        public Task<string> HashPasswordAsync(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Task.FromResult(Convert.ToBase64String(hashedBytes));
        }

        public Task<bool> VerifyPasswordAsync(string password, string hashedPassword)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var hashedInputPassword = Convert.ToBase64String(hashedBytes);
            
            return Task.FromResult(hashedInputPassword == hashedPassword);
        }

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                StrGUID = user.StrGUID,
                StrName = user.StrName,
                StrEmailId = user.StrEmailId,
                BolIsAdmin = user.BolIsAdmin
            };
        }
    }
} 