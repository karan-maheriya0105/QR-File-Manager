using BrochureAPI.Data;
using BrochureAPI.DTOs;
using BrochureAPI.Interfaces;
using BrochureAPI.Models;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System.Text;

namespace BrochureAPI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CategoryService(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            return categories.Select(MapToDto);
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(string id)
        {
            var category = await _context.Categories.FindAsync(id);
            return category != null ? MapToDto(category) : null;
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto, string baseUrl)
        {
            var category = new Category
            {
                StrCategory = createCategoryDto.StrCategory
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // If file is provided, upload it
            if (createCategoryDto.File != null)
            {
                // Create brochures directory if it doesn't exist
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "brochures");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Save file
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(createCategoryDto.File.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await createCategoryDto.File.CopyToAsync(fileStream);
                }

                // Update category
                category.StrFileName = createCategoryDto.File.FileName;
                category.StrFilePath = filePath;

                await _context.SaveChangesAsync();

                // Generate QR code
                await GenerateQRCodeAsync(category.StrGUID, baseUrl);
            }

            return MapToDto(category);
        }

        public async Task<CategoryDto?> UpdateCategoryAsync(string id, UpdateCategoryDto updateCategoryDto, string baseUrl)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return null;

            if (!string.IsNullOrEmpty(updateCategoryDto.StrCategory))
                category.StrCategory = updateCategoryDto.StrCategory;

            // If file is provided, upload it
            if (updateCategoryDto.File != null)
            {
                // Create brochures directory if it doesn't exist
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "brochures");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Delete old file if it exists
                if (!string.IsNullOrEmpty(category.StrFilePath) && File.Exists(category.StrFilePath))
                {
                    File.Delete(category.StrFilePath);
                }

                // Save new file
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(updateCategoryDto.File.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await updateCategoryDto.File.CopyToAsync(fileStream);
                }

                // Update category
                category.StrFileName = updateCategoryDto.File.FileName;
                category.StrFilePath = filePath;

                // Generate QR code
                await GenerateQRCodeAsync(category.StrGUID, baseUrl);
            }

            await _context.SaveChangesAsync();
            return MapToDto(category);
        }

        public async Task<CategoryDto?> UpdateCategoryFileAsync(string id, IFormFile file, string baseUrl)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return null;

            // Create brochures directory if it doesn't exist
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "brochures");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Delete old file if it exists
            if (!string.IsNullOrEmpty(category.StrFilePath) && File.Exists(category.StrFilePath))
            {
                File.Delete(category.StrFilePath);
            }

            // Save new file as {categoryGUID}.pdf
            var fileName = $"{category.StrGUID}.pdf";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Update category
            category.StrFileName = fileName;
            category.StrFilePath = filePath;

            await _context.SaveChangesAsync();

            // Always generate/regenerate QR code after file update
            await GenerateQRCodeAsync(category.StrGUID, baseUrl);

            // Get updated category with QR code
            category = await _context.Categories.FindAsync(id);
            return category != null ? MapToDto(category) : null;
        }

        public async Task<bool> DeleteCategoryAsync(string id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return false;

            // Check if there are any client forms associated with this category
            var hasClientForms = await _context.ClientForms.AnyAsync(cf => cf.StrCategoryGUID == id);
            if (hasClientForms)
                return false;

            // Delete the file if it exists
            if (!string.IsNullOrEmpty(category.StrFilePath) && File.Exists(category.StrFilePath))
            {
                File.Delete(category.StrFilePath);
            }

            // Delete QR code image if it exists
            if (!string.IsNullOrEmpty(category.StrQRCode))
            {
                var qrCodePath = Path.Combine(_environment.WebRootPath, "qrcodes", Path.GetFileName(category.StrQRCode));
                if (File.Exists(qrCodePath))
                {
                    File.Delete(qrCodePath);
                }
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CategoryDto?> UploadBrochureAsync(UploadBrochureDto uploadBrochureDto, string baseUrl)
        {
            var category = await _context.Categories.FindAsync(uploadBrochureDto.CategoryGUID);
            if (category == null)
                return null;

            // Create brochures directory if it doesn't exist
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "brochures");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Delete old file if it exists
            if (!string.IsNullOrEmpty(category.StrFilePath) && File.Exists(category.StrFilePath))
            {
                File.Delete(category.StrFilePath);
            }

            // Save new file as {categoryGUID}.pdf
            var fileName = $"{category.StrGUID}.pdf";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await uploadBrochureDto.File.CopyToAsync(fileStream);
            }

            // Update category
            category.StrFileName = fileName;
            category.StrFilePath = filePath;

            await _context.SaveChangesAsync();

            // Always generate/regenerate QR code after file update
            await GenerateQRCodeAsync(category.StrGUID, baseUrl);

            // Get updated category with QR code
            category = await _context.Categories.FindAsync(uploadBrochureDto.CategoryGUID);
            return category != null ? MapToDto(category) : null;
        }

        public async Task<string> GenerateQRCodeAsync(string categoryId, string baseUrl)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
                throw new ArgumentException("Category not found");

            // Create QR code directory if it doesn't exist
            var qrCodesFolder = Path.Combine(_environment.WebRootPath, "qrcodes");
            if (!Directory.Exists(qrCodesFolder))
            {
                Directory.CreateDirectory(qrCodesFolder);
            }

            // Generate QR code content (URL to the form with category ID)
            var qrCodeContent = $"{baseUrl}/api/ClientForms/form/{categoryId}";

            // Generate QR code
            using var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(qrCodeContent, QRCodeGenerator.ECCLevel.Q);
            
            // Store QR code as a simple text URL for now
            var fileName = $"qr_{categoryId}.txt";
            var qrCodePath = Path.Combine(qrCodesFolder, fileName);
            
            await File.WriteAllTextAsync(qrCodePath, qrCodeContent);

            // Update category with QR code path
            var qrCodeUrl = $"/qrcodes/{fileName}";
            category.StrQRCode = qrCodeUrl;
            await _context.SaveChangesAsync();

            return qrCodeUrl;
        }

        public async Task<CategoryDto?> UpdateCategoryNameAsync(string id, string categoryName)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return null;
            }

            // Update the category name
            category.StrCategory = categoryName;
            
            await _context.SaveChangesAsync();

            return MapToDto(category);
        }

        private static CategoryDto MapToDto(Category category)
        {
            return new CategoryDto
            {
                StrGUID = category.StrGUID,
                StrCategory = category.StrCategory,
                StrFileName = category.StrFileName,
                StrFilePath = category.StrFilePath,
                StrQRCode = category.StrQRCode
            };
        }
    }
}