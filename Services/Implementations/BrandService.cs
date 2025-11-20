using AutoMapper;
using AutoPartInventorySystem.DTOs.Brand;
using AutoPartInventorySystem.Models;
using AutoPartInventorySystem.Repositories.Contracts;
using AutoPartInventorySystem.Services.Contracts;

namespace AutoPartInventorySystem.Services.Implementations
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IStorageService _storageService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public BrandService(
            IBrandRepository brandRepository,
            IStorageService storageService,
            IConfiguration configuration,
            IMapper mapper)
        {
            _brandRepository = brandRepository;
            _storageService = storageService;
            _configuration = configuration;
            _mapper = mapper;
        }
        public async Task<bool> AddAsync(AddBrandDto dto)
        {
            var existing = await _brandRepository.GetByNameAsync(dto.Name);
            if (existing != null) return false;

            string bucket = _configuration["AWS:BucketName"]!;
            string key = $"brands/{Guid.NewGuid()}_{dto.Image.FileName}";

            using var stream = dto.Image.OpenReadStream();
            var uploaded = await _storageService.AddObjectAsync(bucket, key, stream);

            if (!uploaded) return false;

            var brand = _mapper.Map<Brand>(dto);
            brand.ImageUrl = $"https://{bucket}.s3.amazonaws.com/{key}";

            await _brandRepository.AddAsync(brand);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand == null) return false;

            if (!string.IsNullOrWhiteSpace(brand.ImageUrl))
            {
                string bucket = _configuration["AWS:BucketName"]!;
                string key = ExtractKeyFromUrl(brand.ImageUrl);
                await _storageService.DeleteObjectAsync(bucket, key);
            }

            await _brandRepository.DeleteAsync(brand);
            return true;
        }

        public async Task<List<BrandDto>> GetAllBrandsAsync()
        {
            var brands = await _brandRepository.GetAllAsync();
            return _mapper.Map<List<BrandDto>>(brands);
        }

        public async Task<BrandDto?> GetBrandByIdAsync(int id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            return _mapper.Map<BrandDto?>(brand);
        }

        public async Task<bool> UpdateAsync(UpdateBrandDto dto)
        {
            var brand = await _brandRepository.GetByIdAsync(dto.BrandId);
            if (brand == null) return false;

            _mapper.Map(dto, brand);

            if (dto.Image != null)
            {
                string bucket = _configuration["AWS:BucketName"]!;

                if (!string.IsNullOrWhiteSpace(brand.ImageUrl))
                {
                    string oldKey = ExtractKeyFromUrl(brand.ImageUrl);
                    await _storageService.DeleteObjectAsync(bucket, oldKey);
                }

                string newKey = $"brands/{Guid.NewGuid()}_{dto.Image.FileName}";

                using var stream = dto.Image.OpenReadStream();
                var uploaded = await _storageService.AddObjectAsync(bucket, newKey, stream);

                if (!uploaded) return false;

                brand.ImageUrl = $"https://{bucket}.s3.amazonaws.com/{newKey}";
            }

            await _brandRepository.UpdateAsync(brand);
            return true;
        }

        public async Task<bool> UpdateImageAsync(UpdateBrandImageDto dto)
        {
            var brand = await _brandRepository.GetByIdAsync(dto.BrandId);
            if (brand == null) return false;

            string bucket = _configuration["AWS:BucketName"]!;

            if (!string.IsNullOrWhiteSpace(brand.ImageUrl))
            {
                string oldKey = ExtractKeyFromUrl(brand.ImageUrl);
                await _storageService.DeleteObjectAsync(bucket, oldKey);
            }

            string newKey = $"brands/{Guid.NewGuid()}_{dto.Image.FileName}";

            using var stream = dto.Image.OpenReadStream();
            var uploaded = await _storageService.AddObjectAsync(bucket, newKey, stream);

            if (!uploaded) return false;

            brand.ImageUrl = $"https://{bucket}.s3.amazonaws.com/{newKey}";

            await _brandRepository.UpdateAsync(brand);
            return true;
        }

        private string ExtractKeyFromUrl(string url)
        {
            return url.Substring(url.IndexOf(".com/") + 5);
        }
    }
}
