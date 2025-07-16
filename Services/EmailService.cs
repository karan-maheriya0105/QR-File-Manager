using BrochureAPI.Interfaces;
using BrochureAPI.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace BrochureAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly ISmtpSettingsService _smtpSettingsService;
        private readonly ILogger<EmailService> _logger;
        private readonly SmtpSettings _defaultSettings;

        public EmailService(
            ISmtpSettingsService smtpSettingsService, 
            IOptions<SmtpSettings> defaultSettings, 
            ILogger<EmailService> logger)
        {
            _smtpSettingsService = smtpSettingsService;
            _defaultSettings = defaultSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            try
            {
                // Get settings from database or use default
                var settings = await GetSmtpSettingsAsync();
                
                using (var client = CreateSmtpClient(settings))
                {
                    var message = CreateMailMessage(to, subject, body, isHtml, settings);
                    await client.SendMailAsync(message);
                    _logger.LogInformation($"Email sent successfully to {to}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {to}");
                throw;
            }
        }

        public async Task SendEmailWithAttachmentAsync(string to, string subject, string body, string attachmentPath, bool isHtml = true)
        {
            try
            {
                // Get settings from database or use default
                var settings = await GetSmtpSettingsAsync();
                
                using (var client = CreateSmtpClient(settings))
                {
                    var message = CreateMailMessage(to, subject, body, isHtml, settings);
                    
                    if (File.Exists(attachmentPath))
                    {
                        message.Attachments.Add(new Attachment(attachmentPath));
                    }
                    else
                    {
                        _logger.LogWarning($"Attachment file not found: {attachmentPath}");
                    }
                    
                    await client.SendMailAsync(message);
                    _logger.LogInformation($"Email with attachment sent successfully to {to}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email with attachment to {to}");
                throw;
            }
        }

        public async Task SendEmailWithAttachmentAsync(string to, string subject, string body, string attachmentPath, string attachmentName, bool isHtml = true)
        {
            try
            {
                _logger.LogInformation("Attempting to send email with attachment to {To}", to);
                _logger.LogInformation("Attachment path: {Path}", attachmentPath);
                _logger.LogInformation("Attachment name: {Name}", attachmentName);
                
                // Get settings from database or use default
                var settings = await GetSmtpSettingsAsync();
                _logger.LogInformation("Using SMTP server: {Server}:{Port}", settings.Server, settings.Port);
                
                using (var client = CreateSmtpClient(settings))
                {
                    var message = CreateMailMessage(to, subject, body, isHtml, settings);
                    
                    if (File.Exists(attachmentPath))
                    {
                        _logger.LogInformation("File exists, adding attachment");
                        try
                        {
                            var attachment = new Attachment(attachmentPath);
                            attachment.Name = attachmentName; // Set the custom name for the attachment
                            message.Attachments.Add(attachment);
                            _logger.LogInformation("Attachment added successfully");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error adding attachment");
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Attachment file not found: {attachmentPath}");
                    }
                    
                    _logger.LogInformation("Sending email message with subject: {Subject}", subject);
                    await client.SendMailAsync(message);
                    _logger.LogInformation($"Email with named attachment sent successfully to {to}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email with named attachment to {to}");
                throw;
            }
        }



        private async Task<SmtpSettings> GetSmtpSettingsAsync()
        {
            var dbSettings = await _smtpSettingsService.GetSettingsAsync();
            var result = dbSettings ?? _defaultSettings;
            
            _logger.LogInformation("SMTP Settings: Server={Server}, Port={Port}, SSL={SSL}, Username={Username}",
                result.Server, result.Port, result.EnableSsl, result.Username);
                
            if (dbSettings == null)
            {
                _logger.LogWarning("Using default SMTP settings as no settings were found in the database");
            }
            
            return result;
        }

        private SmtpClient CreateSmtpClient(SmtpSettings settings)
        {
            var client = new SmtpClient(settings.Server, settings.Port)
            {
                EnableSsl = settings.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(settings.Username, settings.Password)
            };

            return client;
        }

        private MailMessage CreateMailMessage(string to, string subject, string body, bool isHtml, SmtpSettings settings)
        {
            var message = new MailMessage
            {
                From = new MailAddress(settings.SenderEmail, settings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            message.To.Add(to);
            return message;
        }
    }
}