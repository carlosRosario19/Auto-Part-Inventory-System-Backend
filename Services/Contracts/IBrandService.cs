using AutoPartInventorySystem.DTOs.Brand;

namespace AutoPartInventorySystem.Services.Contracts
{
    public interface IBrandService
    {
        Task<bool> AddAsync(AddBrandDto dto);
        Task<bool> UpdateAsync(UpdateBrandDto dto);
        Task<bool> UpdateImageAsync(UpdateBrandImageDto dto);
        Task<bool> DeleteAsync(int id);
        Task<List<BrandDto>> GetAllBrandsAsync();
        Task<BrandDto?> GetBrandByIdAsync(int id);
    }
}
