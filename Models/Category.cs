using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrochureAPI.Models
{
    public class Category
    {
        [Key]
        public string StrGUID { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [StringLength(100)]
        public string StrCategory { get; set; } = string.Empty;

        [StringLength(255)]
        public string? StrFileName { get; set; }

        [StringLength(500)]
        public string? StrFilePath { get; set; }

        [StringLength(500)]
        public string? StrQRCode { get; set; }
    }
} 