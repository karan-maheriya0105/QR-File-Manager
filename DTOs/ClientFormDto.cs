using System.ComponentModel.DataAnnotations;

namespace BrochureAPI.DTOs
{
    public class ClientFormDto
    {
        public string StrGUID { get; set; } = string.Empty;
        public string StrName { get; set; } = string.Empty;
        public string StrEmailId { get; set; } = string.Empty;
        public string StrPhoneNo { get; set; } = string.Empty;
        public string StrCategoryGUID { get; set; } = string.Empty;
        public string? CategoryName { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class CreateClientFormDto
    {
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
    }
} 