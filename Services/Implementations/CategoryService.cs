using AutoPartInventorySystem.DTOs.Category;
using AutoPartInventorySystem.Models;
using AutoPartInventorySystem.Repositories.Contracts;
using AutoPartInventorySystem.Services.Contracts;

namespace AutoPartInventorySystem.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IStorageService _storageService;
        private readonly IConfiguration _configuration;

        public CategoryService(
            ICategoryRepository categoryRepository, 
            IStorageService storageService,
            IConfiguration configuration)
        {
            _categoryRepository = categoryRepository;
            _storageService = storageService;
            _configuration = configuration;
        }

        public async Task<bool> AddAsync(AddCategoryDto dto)
        {
            // Ensure name is unique
            var existing = await _categoryRepository.GetByNameAsync(dto.Name);
            if (existing != null)
                return false;

            // Prepare S3 upload
            string bucketName = _configuration["AWS:BucketName"]!;
            string key = $"categories/{Guid.NewGuid()}_{dto.Image.FileName}";

            using var stream = dto.Image.OpenReadStream();
            var uploaded = await _storageService.AddObjectAsync(bucketName, key, stream);

            if (!uploaded) return false;

            string imageUrl = $"https://{bucketName}.s3.amazonaws.com/{key}";

            var category = new Category
            {
                Name = dto.Name,
                ImageUrl = imageUrl
            };

            await _categoryRepository.AddAsync(category);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return false;

            // Delete image from S3
            if (!string.IsNullOrWhiteSpace(category.ImageUrl))
            {
                string bucket = _configuration["AWS:BucketName"]!;
                string key = ExtractKeyFromUrl(category.ImageUrl);

                await _storageService.DeleteObjectAsync(bucket, key);
            }

            await _categoryRepository.DeleteAsync(category);
            return true;
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();

            return categories
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    ImageUrl = c.ImageUrl
                })
                .ToList();
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return null;

            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                ImageUrl = category.ImageUrl
            };
        }

        public async Task<bool> UpdateAsync(UpdateCategoryDto dto)
        {
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null)
                return false;

            // If there's a new image, replace it
            if (dto.Image != null)
            {
                string bucketName = _configuration["AWS:BucketName"]!;

                // Delete the old image if it exists
                if (!string.IsNullOrWhiteSpace(category.ImageUrl))
                {
                    string oldKey = ExtractKeyFromUrl(category.ImageUrl);
                    await _storageService.DeleteObjectAsync(bucketName, oldKey);
                }

                // Upload the new image
                string newKey = $"categories/{Guid.NewGuid()}_{dto.Image.FileName}";
                using var stream = dto.Image.OpenReadStream();
                var uploaded = await _storageService.AddObjectAsync(bucketName, newKey, stream);

                if (!uploaded)
                    return false;

                category.ImageUrl = $"https://{bucketName}.s3.amazonaws.com/{newKey}";
            }

            // Update name
            category.Name = dto.Name;

            await _categoryRepository.UpdateAsync(category);
            return true;
        }

        public async Task<bool> UpdateImageAsync(UpdateCategoryImageDto dto)
        {
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null)
                return false;

            string bucketName = _configuration["AWS:BucketName"]!;

            // Delete old image if it exists
            if (!string.IsNullOrWhiteSpace(category.ImageUrl))
            {
                string oldKey = ExtractKeyFromUrl(category.ImageUrl);
                await _storageService.DeleteObjectAsync(bucketName, oldKey);
            }

            // Upload new image
            string newKey = $"categories/{Guid.NewGuid()}_{dto.Image.FileName}";
            using var stream = dto.Image.OpenReadStream();
            var uploaded = await _storageService.AddObjectAsync(bucketName, newKey, stream);

            if (!uploaded)
                return false;

            // Update database entity with new URL
            category.ImageUrl = $"https://{bucketName}.s3.amazonaws.com/{newKey}";

            await _categoryRepository.UpdateAsync(category);
            return true;
        }

        // Helper to convert S3 URL -> key
        private string ExtractKeyFromUrl(string url)
        {
            // Example: https://bucket.s3.amazonaws.com/categories/abc.png
            return url.Substring(url.IndexOf(".com/") + 5);
        }
    }
}
