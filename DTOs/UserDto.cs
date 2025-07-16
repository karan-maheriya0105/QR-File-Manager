using System.ComponentModel.DataAnnotations;

namespace BrochureAPI.DTOs
{
    public class UserDto
    {
        public string StrGUID { get; set; } = string.Empty;
        public string StrName { get; set; } = string.Empty;
        public string StrEmailId { get; set; } = string.Empty;
        public bool BolIsAdmin { get; set; }
    }

    public class CreateUserDto
    {
        [Required]
        [StringLength(100)]
        public string StrName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string StrEmailId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [MinLength(6)]
        public string StrPassword { get; set; } = string.Empty;

        public bool BolIsAdmin { get; set; } = false;
    }

    public class UpdateUserDto
    {
        [StringLength(100)]
        public string? StrName { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? StrEmailId { get; set; }

        [StringLength(100)]
        [MinLength(6)]
        public string? StrPassword { get; set; }

        public bool? BolIsAdmin { get; set; }
    }

    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string StrEmailId { get; set; } = string.Empty;

        [Required]
        public string StrPassword { get; set; } = string.Empty;
    }
} 