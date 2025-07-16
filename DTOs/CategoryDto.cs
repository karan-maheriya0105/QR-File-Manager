using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace BrochureAPI.DTOs
{
    public class CategoryDto
    {
        public string StrGUID { get; set; } = string.Empty;
        public string StrCategory { get; set; } = string.Empty;
        public string? StrFileName { get; set; }
        public string? StrFilePath { get; set; }
        public string? StrQRCode { get; set; }
    }

    public class CreateCategoryDto
    {
        [Required]
        [StringLength(100)]
        public string StrCategory { get; set; } = string.Empty;
        
        // Optional file upload with category creation
        public IFormFile? File { get; set; }
    }

    public class UpdateCategoryDto
    {
        [StringLength(100)]
        public string? StrCategory { get; set; }
        
        // Optional file upload with category update
        public IFormFile? File { get; set; }
    }

    public class UploadBrochureDto
    {
        [Required]
        public string CategoryGUID { get; set; } = string.Empty;

        [Required]
        public IFormFile File { get; set; } = null!;
    }
} 