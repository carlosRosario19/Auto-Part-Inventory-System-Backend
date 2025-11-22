using AutoMapper;
using AutoPartInventorySystem.DTOs.AutoPart;
using AutoPartInventorySystem.Models;
using AutoPartInventorySystem.Repositories.Contracts;
using AutoPartInventorySystem.Services.Contracts;
using AutoPartInventorySystem.Util;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AutoPartInventorySystem.Services.Implementations
{
    public class AutoPartService : IAutoPartService
    {
        private readonly IAutoPartRepository _autoPartRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly ILogRepository _logRepository;
        private readonly IStorageService _storageService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AutoPartService(
            IAutoPartRepository autoPartRepository,
            ICategoryRepository categoryRepository,
            IBrandRepository brandRepository,
            IVehicleRepository vehicleRepository,
            ILogRepository logRepository,
            IStorageService storageService,
            IConfiguration configuration,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _autoPartRepository = autoPartRepository;
            _categoryRepository = categoryRepository;
            _brandRepository = brandRepository;
            _vehicleRepository = vehicleRepository;
            _logRepository = logRepository;
            _storageService = storageService;
            _configuration = configuration;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }


        // ------------------------------------------------------------
        // ADD NEW AUTO-PART
        // ------------------------------------------------------------
        public async Task<bool> AddAsync(AddAutoPartDto dto)
        {
            // Validate Category
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null)
                return false;

            // Validate Brand list
            var brands = await _brandRepository.GetByIdsAsync(dto.BrandIds);

            // Ensure all BrandIds are valid
            if (brands.Count != dto.BrandIds.Count)
                return false;

            string bucket = _configuration["AWS:BucketName"]!;
            string key = $"auto-parts/{Guid.NewGuid()}_{dto.Image.FileName}";

            using var stream = dto.Image.OpenReadStream();
            var uploaded = await _storageService.AddObjectAsync(bucket, key, stream);
            if (!uploaded)
                return false;

            var autoPart = _mapper.Map<AutoPart>(dto);
            autoPart.ImageUrl = $"https://{bucket}.s3.amazonaws.com/{key}";
            autoPart.UpdatedAt = DateTime.UtcNow;

            // Assign ALL brands
            autoPart.Brands = brands;

            await _autoPartRepository.AddAsync(autoPart);

            // Log action
            await _logRepository.AddLogAsync(new Log
            {
                PK = $"AutoPart#{autoPart.AutoPartId}",
                SK = DateTime.UtcNow.ToString("o"),
                Username = GetUsername(),
                Action = "Created",
                EntityType = "AutoPart",
                EntityId = autoPart.AutoPartId,
                Metadata = new Dictionary<string, object>
            {
                { "Name", autoPart.Name },
                { "BrandIds", dto.BrandIds },
                { "CategoryId", dto.CategoryId },
                { "ImageUrl", autoPart.ImageUrl }
            }
            });

            return true;
        }
        // ------------------------------------------------------------
        // DELETE AUTO-PART
        // ------------------------------------------------------------
        public async Task<bool> DeleteAsync(int id)
        {
            var autoPart = await _autoPartRepository.GetByIdAsync(id);
            if (autoPart == null)
                return false;

            // Delete image from S3
            if (!string.IsNullOrWhiteSpace(autoPart.ImageUrl))
            {
                string bucket = _configuration["AWS:BucketName"]!;
                string key = ExtractKeyFromUrl(autoPart.ImageUrl);
                await _storageService.DeleteObjectAsync(bucket, key);
            }

            await _autoPartRepository.DeleteAsync(autoPart);

            // Log delete
            await _logRepository.AddLogAsync(new Log
            {
                PK = $"AutoPart#{id}",
                SK = DateTime.UtcNow.ToString("o"),
                Username = GetUsername(),
                Action = "Deleted",
                EntityType = "AutoPart",
                EntityId = id,
                Metadata = new Dictionary<string, object>
            {
                { "Name", autoPart.Name },
                { "ImageUrl", autoPart.ImageUrl }
            }
            });

            return true;
        }

        // ------------------------------------------------------------
        // GET ALL WITH FILTERS + PAGINATION
        // ------------------------------------------------------------
        public async Task<PagedResult<AutoPartDto>> GetAllAsync(AutoPartQueryDto query)
        {
            var result = await _autoPartRepository.GetAllAsync(query);

            return new PagedResult<AutoPartDto>
            {
                Items = _mapper.Map<IEnumerable<AutoPartDto>>(result.Items),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        // ------------------------------------------------------------
        // GET BY ID
        // ------------------------------------------------------------
        public async Task<AutoPartDto?> GetAutoPartByIdAsync(int id)
        {
            var autoPart = await _autoPartRepository.GetByIdAsync(id);
            return autoPart == null ? null : _mapper.Map<AutoPartDto>(autoPart);
        }

        public async Task<LinkVehicleResult> LinkVehicleAsync(LinkPartWithVehicleDto dto)
        {
            // Validate years
            if (dto.EndYear != null && dto.StartYear > dto.EndYear)
                return LinkVehicleResult.InvalidYearRange;


            // Validate auto-part exists
            var autoPart = await _autoPartRepository.GetByIdAsync(dto.AutoPartId);
            if (autoPart == null)
                return LinkVehicleResult.AutoPartNotFound;

            // Validate brand
            var brand = await _brandRepository.GetByIdAsync(dto.BrandId);
            if (brand == null)
                return LinkVehicleResult.BrandNotFound;

            // Check if a vehicle with same specs already exists
            var vehicle = await _vehicleRepository.FindExistingVehicleAsync(
                dto.BrandId,
                dto.Model,
                dto.StartYear,
                dto.EndYear
            );

            // If it does NOT exist → create it
            if (vehicle == null)
            {
                vehicle = new Vehicle
                {
                    BrandId = dto.BrandId,
                    Model = dto.Model,
                    StartYear = dto.StartYear,
                    EndYear = dto.EndYear
                };

                await _vehicleRepository.AddAsync(vehicle);
            }

            // If already linked → nothing to do
            if (autoPart.Vehicles.Any(v => v.VehicleId == vehicle.VehicleId))
                return LinkVehicleResult.AlreadyLinked;

            // Link vehicle to auto-part
            autoPart.Vehicles.Add(vehicle);

            await _autoPartRepository.UpdateAsync(autoPart);
            return LinkVehicleResult.Success;
        }

        // ------------------------------------------------------------
        // UPDATE AUTO-PART
        // ------------------------------------------------------------
        public async Task<bool> UpdateAsync(UpdateAutoPartDto dto)
        {
            var autoPart = await _autoPartRepository.GetByIdAsync(dto.AutoPartId);
            if (autoPart == null)
                return false;

            var oldMetadata = new Dictionary<string, object>
            {
                { "Name", autoPart.Name },
                { "ImageUrl", autoPart.ImageUrl },
                { "BrandIds", autoPart.Brands.Select(b => b.BrandId).ToList() },
                { "CategoryId", autoPart.CategoryId }
            };

            // Validate category
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null)
                return false;

            // Validate all brands
            var brands = await _brandRepository.GetByIdsAsync(dto.BrandIds);
            if (brands.Count != dto.BrandIds.Count)
                return false;

            // Map scalar fields
            _mapper.Map(dto, autoPart);
            autoPart.UpdatedAt = DateTime.UtcNow;

            // Replace brand relationships
            autoPart.Brands.Clear();
            foreach (var brand in brands)
                autoPart.Brands.Add(brand);

            // Replace image if provided
            if (dto.Image != null)
            {
                string bucket = _configuration["AWS:BucketName"]!;

                if (!string.IsNullOrWhiteSpace(autoPart.ImageUrl))
                {
                    string oldKey = ExtractKeyFromUrl(autoPart.ImageUrl);
                    await _storageService.DeleteObjectAsync(bucket, oldKey);
                }

                string newKey = $"auto-parts/{Guid.NewGuid()}_{dto.Image.FileName}";
                using var stream = dto.Image.OpenReadStream();

                if (!await _storageService.AddObjectAsync(bucket, newKey, stream))
                    return false;

                autoPart.ImageUrl = $"https://{bucket}.s3.amazonaws.com/{newKey}";
            }

            await _autoPartRepository.UpdateAsync(autoPart);

            // Log update
            await _logRepository.AddLogAsync(new Log
            {
                PK = $"AutoPart#{autoPart.AutoPartId}",
                SK = DateTime.UtcNow.ToString("o"),
                Username = GetUsername(),
                Action = "Updated",
                EntityType = "AutoPart",
                EntityId = autoPart.AutoPartId,
                Metadata = new Dictionary<string, object>
            {
                { "Before", oldMetadata },
                { "After", new {
                    autoPart.Name,
                    autoPart.ImageUrl,
                    BrandIds = autoPart.Brands.Select(b => b.BrandId).ToList(),
                    autoPart.CategoryId
                }}
            }
            });

            return true;
        }

        // ------------------------------------------------------------
        // Helper: extract S3 key from full image URL
        // ------------------------------------------------------------
        private string ExtractKeyFromUrl(string url)
        {
            return url.Substring(url.IndexOf(".com/") + 5);
        }

        private string GetUsername()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null || httpContext.User.Identity == null || !httpContext.User.Identity.IsAuthenticated)
                return "Unknown";

            // Try claims in order of relevance based on your JWT
            return httpContext.User.FindFirst("email")?.Value
                   ?? httpContext.User.FindFirst(JwtRegisteredClaimNames.Email)?.Value
                   ?? httpContext.User.FindFirst(ClaimTypes.Email)?.Value
                   ?? "Unknown";
        }
    }
}
