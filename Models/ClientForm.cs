using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrochureAPI.Models
{
    public class ClientForm
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
        [StringLength(20)]
        public string StrPhoneNo { get; set; } = string.Empty;

        [Required]
        public string StrCategoryGUID { get; set; } = string.Empty;

        [ForeignKey("StrCategoryGUID")]
        public Category? Category { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
} 