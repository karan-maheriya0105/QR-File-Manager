using System.ComponentModel.DataAnnotations;

namespace BrochureAPI.Models
{
    public class SmtpSettings
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Server { get; set; } = string.Empty;
        
        [Required]
        public int Port { get; set; }
        
        [Required]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        public string SenderEmail { get; set; } = string.Empty;
        
        [Required]
        public string SenderName { get; set; } = string.Empty;
        
        public bool EnableSsl { get; set; }
    }
} 