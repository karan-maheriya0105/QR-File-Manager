using BrochureAPI.Data;
using BrochureAPI.Interfaces;
using BrochureAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BrochureAPI.Services
{
    public class SmtpSettingsService : ISmtpSettingsService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SmtpSettingsService> _logger;

        public SmtpSettingsService(ApplicationDbContext context, ILogger<SmtpSettingsService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<SmtpSettings?> GetSettingsAsync()
        {
            try
            {
                // Always get the first record (we only maintain one SMTP settings record)
                return await _context.SmtpSettings.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving SMTP settings");
                throw;
            }
        }

        public async Task<SmtpSettings> SaveSettingsAsync(SmtpSettings settings)
        {
            try
            {
                // Check if settings already exist
                var existingSettings = await _context.SmtpSettings.FirstOrDefaultAsync();
                
                if (existingSettings != null)
                {
                    // Update existing settings
                    return await UpdateSettingsAsync(settings);
                }
                
                // Create new settings
                _context.SmtpSettings.Add(settings);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("SMTP settings saved successfully");
                return settings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving SMTP settings");
                throw;
            }
        }

        public async Task<SmtpSettings> UpdateSettingsAsync(SmtpSettings settings)
        {
            try
            {
                var existingSettings = await _context.SmtpSettings.FirstOrDefaultAsync();
                
                if (existingSettings == null)
                {
                    _logger.LogWarning("No SMTP settings found to update");
                    return await SaveSettingsAsync(settings);
                }
                
                // Update properties
                existingSettings.Server = settings.Server;
                existingSettings.Port = settings.Port;
                existingSettings.Username = settings.Username;
                existingSettings.Password = settings.Password;
                existingSettings.SenderEmail = settings.SenderEmail;
                existingSettings.SenderName = settings.SenderName;
                existingSettings.EnableSsl = settings.EnableSsl;
                
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("SMTP settings updated successfully");
                return existingSettings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating SMTP settings");
                throw;
            }
        }
    }
} 