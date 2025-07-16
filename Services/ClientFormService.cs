using BrochureAPI.Data;
using BrochureAPI.DTOs;
using BrochureAPI.Interfaces;
using BrochureAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace BrochureAPI.Services
{
    public class ClientFormService : IClientFormService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<ClientFormService> _logger;

        public ClientFormService(
            ApplicationDbContext context, 
            IEmailService emailService,
            ILogger<ClientFormService> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<IEnumerable<ClientFormDto>> GetAllClientFormsAsync()
        {
            var clientForms = await _context.ClientForms
                .Include(cf => cf.Category)
                .ToListAsync();

            return clientForms.Select(MapToDto);
        }

        public async Task<ClientFormDto?> GetClientFormByIdAsync(string id)
        {
            var clientForm = await _context.ClientForms
                .Include(cf => cf.Category)
                .FirstOrDefaultAsync(cf => cf.StrGUID == id);

            return clientForm != null ? MapToDto(clientForm) : null;
        }

        public async Task<IEnumerable<ClientFormDto>> GetClientFormsByCategoryAsync(string categoryId)
        {
            var clientForms = await _context.ClientForms
                .Include(cf => cf.Category)
                .Where(cf => cf.StrCategoryGUID == categoryId)
                .ToListAsync();

            return clientForms.Select(MapToDto);
        }

        public async Task<ClientFormDto> CreateClientFormAsync(CreateClientFormDto createClientFormDto)
        {
            // Check if the category exists
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.StrGUID == createClientFormDto.StrCategoryGUID);
                
            if (category == null)
            {
                throw new ArgumentException("Category not found");
            }

            var clientForm = new ClientForm
            {
                StrName = createClientFormDto.StrName,
                StrEmailId = createClientFormDto.StrEmailId,
                StrPhoneNo = createClientFormDto.StrPhoneNo,
                StrCategoryGUID = createClientFormDto.StrCategoryGUID,
                CreatedDate = DateTime.UtcNow
            };

            _context.ClientForms.Add(clientForm);
            await _context.SaveChangesAsync();

            // Send email with brochure information
            try
            {
                await SendBrochureEmailAsync(clientForm, category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send brochure email to {Email}", clientForm.StrEmailId);
                // We don't want to fail the form submission if email sending fails
            }

            // Fetch the complete client form with category
            var createdClientForm = await _context.ClientForms
                .Include(cf => cf.Category)
                .FirstOrDefaultAsync(cf => cf.StrGUID == clientForm.StrGUID);

            return MapToDto(createdClientForm!);
        }

        public async Task<bool> DeleteClientFormAsync(string id)
        {
            var clientForm = await _context.ClientForms.FindAsync(id);
            if (clientForm == null)
                return false;

            _context.ClientForms.Remove(clientForm);
            await _context.SaveChangesAsync();
            return true;
        }

        private static ClientFormDto MapToDto(ClientForm clientForm)
        {
            return new ClientFormDto
            {
                StrGUID = clientForm.StrGUID,
                StrName = clientForm.StrName,
                StrEmailId = clientForm.StrEmailId,
                StrPhoneNo = clientForm.StrPhoneNo,
                StrCategoryGUID = clientForm.StrCategoryGUID,
                CategoryName = clientForm.Category?.StrCategory,
                CreatedDate = clientForm.CreatedDate
            };
        }

        private async Task SendBrochureEmailAsync(ClientForm clientForm, Category category)
        {
            string subject = $"Your Requested Information: {category.StrCategory}";
            
            // Create email body with brochure information
            string emailBody = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #4a6cf7; color: white; padding: 10px 20px; text-align: center; }}
                        .content {{ padding: 20px; background-color: #f9f9f9; }}
                        .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #666; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Thank You for Your Interest</h1>
                        </div>
                        <div class='content'>
                            <p>Dear {clientForm.StrName},</p>
                            <p>Thank you for requesting information about <strong>{category.StrCategory}</strong>.</p>
                            <p>We have received your request and are pleased to provide you with the requested information.</p>
                            <p>If you have any questions or need additional information, please don't hesitate to contact us.</p>
                            <p>Best regards,<br/>The Team</p>
                        </div>
                        <div class='footer'>
                            <p>This email was sent to {clientForm.StrEmailId} in response to your request.</p>
                        </div>
                    </div>
                </body>
                </html>";

            // Check if category has a brochure file path
            if (!string.IsNullOrEmpty(category.StrFilePath) && File.Exists(category.StrFilePath))
            {
                // Get attachment filename - use the original filename if available, otherwise use a generic name
                string attachmentName = !string.IsNullOrEmpty(category.StrFileName) 
                    ? category.StrFileName 
                    : $"{category.StrCategory} Brochure.pdf";

                // Add text to email body to indicate there's an attachment
                emailBody = emailBody.Replace("</p>\n                            <p>Best regards", 
                    "</p>\n                            <p><strong>Please find the requested brochure attached to this email.</strong></p>\n                            <p>Best regards");

                // Send email with attachment
                await _emailService.SendEmailWithAttachmentAsync(
                    clientForm.StrEmailId,
                    subject,
                    emailBody,
                    category.StrFilePath,
                    attachmentName
                );
                _logger.LogInformation("Sent brochure email with attachment to {Email}", clientForm.StrEmailId);
            }
            else
            {
                // Send email without attachment
                await _emailService.SendEmailAsync(
                    clientForm.StrEmailId,
                    subject,
                    emailBody
                );
                _logger.LogInformation("Sent brochure email without attachment to {Email}", clientForm.StrEmailId);
            }
        }
    }
} 