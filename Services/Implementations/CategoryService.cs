using AutoMapper;
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
        private readonly IMapper _mapper;

        public CategoryService(
            ICategoryRepository categoryRepository, 
            IStorageService storageService,
            IConfiguration configuration,
            IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _storageService = storageService;
            _configuration = configuration;
            _mapper = mapper;
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

            // Map dto to entity
            var category = _mapper.Map<Category>(dto);
            category.ImageUrl = $"https://{bucketName}.s3.amazonaws.com/{key}";

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
            return _mapper.Map<List<CategoryDto>>(categories);
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return category == null ? null : _mapper.Map<CategoryDto>(category);
        }

        public async Task<bool> UpdateAsync(UpdateCategoryDto dto)
        {
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null)
                return false;

            // Map name (AutoMapper)
            _mapper.Map(dto, category);

            string bucket = _configuration["AWS:BucketName"]!;

            // If new image is provided, replace it
            if (dto.Image != null)
            {
                if (!string.IsNullOrWhiteSpace(category.ImageUrl))
                {
                    string oldKey = ExtractKeyFromUrl(category.ImageUrl);
                    await _storageService.DeleteObjectAsync(bucket, oldKey);
                }

                string newKey = $"categories/{Guid.NewGuid()}_{dto.Image.FileName}";
                using var stream = dto.Image.OpenReadStream();

                if (!await _storageService.AddObjectAsync(bucket, newKey, stream))
                    return false;

                category.ImageUrl = $"https://{bucket}.s3.amazonaws.com/{newKey}";
            }

            await _categoryRepository.UpdateAsync(category);
            return true;
        }

        public async Task<bool> UpdateImageAsync(UpdateCategoryImageDto dto)
        {
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null)
                return false;

            string bucket = _configuration["AWS:BucketName"]!;

            if (!string.IsNullOrWhiteSpace(category.ImageUrl))
            {
                string oldKey = ExtractKeyFromUrl(category.ImageUrl);
                await _storageService.DeleteObjectAsync(bucket, oldKey);
            }

            string newKey = $"categories/{Guid.NewGuid()}_{dto.Image.FileName}";
            using var stream = dto.Image.OpenReadStream();

            if (!await _storageService.AddObjectAsync(bucket, newKey, stream))
                return false;

            category.ImageUrl = $"https://{bucket}.s3.amazonaws.com/{newKey}";

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
