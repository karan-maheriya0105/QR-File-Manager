using BrochureAPI.DTOs;
using BrochureAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrochureAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientFormsController : ControllerBase
    {
        private readonly IClientFormService _clientFormService;

        public ClientFormsController(IClientFormService clientFormService)
        {
            _clientFormService = clientFormService;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<IEnumerable<ClientFormDto>>> GetClientForms()
        {
            var clientForms = await _clientFormService.GetAllClientFormsAsync();
            return Ok(clientForms);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ClientFormDto>> GetClientForm(string id)
        {
            var clientForm = await _clientFormService.GetClientFormByIdAsync(id);
            if (clientForm == null)
            {
                return NotFound();
            }
            return Ok(clientForm);
        }

        [HttpGet("category/{categoryId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<IEnumerable<ClientFormDto>>> GetClientFormsByCategory(string categoryId)
        {
            var clientForms = await _clientFormService.GetClientFormsByCategoryAsync(categoryId);
            return Ok(clientForms);
        }

        [HttpPost]
        // No authorization required for form submission
        public async Task<ActionResult<ClientFormDto>> CreateClientForm(CreateClientFormDto createClientFormDto)
        {
            try
            {
                var clientForm = await _clientFormService.CreateClientFormAsync(createClientFormDto);
                return CreatedAtAction(nameof(GetClientForm), new { id = clientForm.StrGUID }, clientForm);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteClientForm(string id)
        {
            var result = await _clientFormService.DeleteClientFormAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        // Endpoint for QR code scanning to submit form
        [HttpGet("form/{categoryId}")]
        // No authorization required for form access
        public ActionResult GetForm(string categoryId)
        {
            // This would typically return a view or redirect to a form page
            // For API purposes, we'll return the category ID that the client needs to include in their form submission
            return Ok(new { CategoryId = categoryId, Message = "Please submit your information using the form" });
        }
    }
} 