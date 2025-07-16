using BrochureAPI.DTOs;
using BrochureAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrochureAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(string id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<CategoryDto>> CreateCategory([FromForm] CreateCategoryDto createCategoryDto)
        {
            try
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var category = await _categoryService.CreateCategoryAsync(createCategoryDto, baseUrl);
                return CreatedAtAction(nameof(GetCategory), new { id = category.StrGUID }, category);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateCategory(string id, [FromForm] UpdateCategoryDto updateCategoryDto)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            
            // If we have a file in the DTO, update the category file
            if (updateCategoryDto.File != null)
            {
                var category = await _categoryService.UpdateCategoryFileAsync(id, updateCategoryDto.File, baseUrl);
                if (category == null)
                {
                    return NotFound();
                }
                return Ok(category);
            }
            
            // If we have category name but no file, update the category name
            if (!string.IsNullOrEmpty(updateCategoryDto.StrCategory))
            {
                // You'll need to implement this method in your service
                var category = await _categoryService.UpdateCategoryNameAsync(id, updateCategoryDto.StrCategory);
                if (category == null)
                {
                    return NotFound();
                }
                return Ok(category);
            }
            
            return BadRequest("No update data provided");
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result)
            {
                return NotFound("Category not found or has associated client forms");
            }
            return NoContent();
        }

        [HttpPost("upload-brochure")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<CategoryDto>> UploadBrochure([FromForm] UploadBrochureDto uploadBrochureDto)
        {
            try
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var category = await _categoryService.UploadBrochureAsync(uploadBrochureDto, baseUrl);
                if (category == null)
                {
                    return NotFound("Category not found");
                }
                
                return Ok(category);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}/generate-qr")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<string>> GenerateQRCode(string id)
        {
            try
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var qrCodeUrl = await _categoryService.GenerateQRCodeAsync(id, baseUrl);
                return Ok(new { QrCodeUrl = qrCodeUrl });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
} 