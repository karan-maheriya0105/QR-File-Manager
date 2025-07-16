using BrochureAPI.Interfaces;
using BrochureAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrochureAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class SmtpSettingsController : ControllerBase
    {
        private readonly ISmtpSettingsService _smtpSettingsService;
        private readonly ILogger<SmtpSettingsController> _logger;

        public SmtpSettingsController(ISmtpSettingsService smtpSettingsService, ILogger<SmtpSettingsController> logger)
        {
            _smtpSettingsService = smtpSettingsService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<SmtpSettings>> GetSettings()
        {
            try
            {
                var settings = await _smtpSettingsService.GetSettingsAsync();
                if (settings == null)
                {
                    return NotFound("No SMTP settings configured");
                }
                
                // Don't return the password for security reasons
                settings.Password = "";
                
                return Ok(settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving SMTP settings");
                return StatusCode(500, "Failed to retrieve SMTP settings");
            }
        }

        [HttpPost]
        public async Task<ActionResult<SmtpSettings>> SaveSettings([FromBody] SmtpSettings settings)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var savedSettings = await _smtpSettingsService.SaveSettingsAsync(settings);
                
                // Don't return the password for security reasons
                savedSettings.Password = "";
                
                return Ok(savedSettings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving SMTP settings");
                return StatusCode(500, "Failed to save SMTP settings");
            }
        }

        [HttpPut]
        public async Task<ActionResult<SmtpSettings>> UpdateSettings([FromBody] SmtpSettings settings)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedSettings = await _smtpSettingsService.UpdateSettingsAsync(settings);
                
                // Don't return the password for security reasons
                updatedSettings.Password = "";
                
                return Ok(updatedSettings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating SMTP settings");
                return StatusCode(500, "Failed to update SMTP settings");
            }
        }
    }
} 