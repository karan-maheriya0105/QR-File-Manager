using BrochureAPI.DTOs;
using Microsoft.AspNetCore.Http;

namespace BrochureAPI.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto?> GetCategoryByIdAsync(string id);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto, string baseUrl);
        Task<CategoryDto?> UpdateCategoryAsync(string id, UpdateCategoryDto updateCategoryDto, string baseUrl);
        Task<CategoryDto?> UpdateCategoryFileAsync(string id, IFormFile file, string baseUrl);
        Task<CategoryDto?> UpdateCategoryNameAsync(string id, string categoryName);
        Task<bool> DeleteCategoryAsync(string id);
        Task<CategoryDto?> UploadBrochureAsync(UploadBrochureDto uploadBrochureDto, string baseUrl);
        Task<string> GenerateQRCodeAsync(string categoryId, string baseUrl);
    }
}