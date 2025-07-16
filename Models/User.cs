using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrochureAPI.Models
{
    public class User
    {
        [Key]
        public string StrGUID { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [StringLength(100)]
        public string StrName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string StrEmailId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string StrPassword { get; set; } = string.Empty;

        public bool BolIsAdmin { get; set; } = false;
        
        // JWT Authentication properties
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
} 