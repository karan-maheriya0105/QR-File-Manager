using BrochureAPI.Interfaces;
using BrochureAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrochureAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ISmtpSettingsService _smtpSettingsService;
        private readonly ILogger<EmailController> _logger;

        public EmailController(
            IEmailService emailService, 
            ISmtpSettingsService smtpSettingsService,
            ILogger<EmailController> logger)
        {
            _emailService = emailService;
            _smtpSettingsService = smtpSettingsService;
            _logger = logger;
        }

        [HttpPost("send")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
        {
            if (string.IsNullOrEmpty(request.To) || string.IsNullOrEmpty(request.Subject) || string.IsNullOrEmpty(request.Body))
            {
                return BadRequest("Email address, subject, and body are required.");
            }

            try
            {
                await _emailService.SendEmailAsync(request.To, request.Subject, request.Body, request.IsHtml);
                return Ok(new { Message = "Email sent successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email");
                return StatusCode(500, "Failed to send email. Please check the SMTP settings.");
            }
        }

        [HttpPost("send-with-attachment")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> SendEmailWithAttachment([FromBody] EmailWithAttachmentRequest request)
        {
            if (string.IsNullOrEmpty(request.To) || string.IsNullOrEmpty(request.Subject) || 
                string.IsNullOrEmpty(request.Body) || string.IsNullOrEmpty(request.AttachmentPath))
            {
                return BadRequest("Email address, subject, body, and attachment path are required.");
            }

            try
            {
                await _emailService.SendEmailWithAttachmentAsync(
                    request.To, 
                    request.Subject, 
                    request.Body, 
                    request.AttachmentPath, 
                    request.IsHtml);
                
                return Ok(new { Message = "Email with attachment sent successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email with attachment");
                return StatusCode(500, "Failed to send email with attachment. Please check the SMTP settings and attachment path.");
            }
        }

        [HttpPost("test")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> TestEmail()
        {
            try
            {
                // Get SMTP settings to use the configured email address
                var smtpSettings = await _smtpSettingsService.GetSettingsAsync();
                if (smtpSettings == null)
                {
                    return BadRequest("SMTP settings not configured. Please configure SMTP settings first.");
                }

                var recipientEmail = smtpSettings.Username; // Use the configured username as recipient
                
                var testEmailBody = @"
                    <h2>SMTP Test Email</h2>
                    <p>This is a test email to verify your SMTP settings are working correctly.</p>
                    <p>If you received this email, your SMTP configuration is correct!</p>
                    <hr>
                    <p>Sent from QR Code Manager Application</p>";
                
                await _emailService.SendEmailAsync(
                    recipientEmail, 
                    "SMTP Test Email", 
                    testEmailBody);
                
                return Ok(new { Message = $"Test email sent successfully to {recipientEmail}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending test email");
                return StatusCode(500, $"Failed to send test email: {ex.Message}");
            }
        }


    }

    public class EmailRequest
    {
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsHtml { get; set; } = true;
    }

    public class EmailWithAttachmentRequest : EmailRequest
    {
        public string AttachmentPath { get; set; } = string.Empty;
    }
}