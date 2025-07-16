using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrochureAPI.Models
{
    public class CompanyProfile
    {
        [Key]
        public string StrGUID { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [StringLength(100)]
        public string StrCompanyName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? StrDescription { get; set; }

        [StringLength(200)]
        public string? StrAddress { get; set; }

        [StringLength(50)]
        public string? StrPhone { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? StrEmail { get; set; }

        [StringLength(200)]
        public string? StrWebsite { get; set; }

        [StringLength(200)]
        public string? StrLogoPath { get; set; }
    }
} 