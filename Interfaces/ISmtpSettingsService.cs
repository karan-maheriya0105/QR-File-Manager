using BrochureAPI.Models;

namespace BrochureAPI.Interfaces
{
    public interface ISmtpSettingsService
    {
        /// <summary>
        /// Gets the current SMTP settings
        /// </summary>
        /// <returns>The current SMTP settings or null if not configured</returns>
        Task<SmtpSettings?> GetSettingsAsync();
        
        /// <summary>
        /// Saves SMTP settings
        /// </summary>
        /// <param name="settings">The SMTP settings to save</param>
        /// <returns>The saved SMTP settings</returns>
        Task<SmtpSettings> SaveSettingsAsync(SmtpSettings settings);
        
        /// <summary>
        /// Updates existing SMTP settings
        /// </summary>
        /// <param name="settings">The updated SMTP settings</param>
        /// <returns>The updated SMTP settings</returns>
        Task<SmtpSettings> UpdateSettingsAsync(SmtpSettings settings);
    }
} 