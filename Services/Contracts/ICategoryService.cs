using AutoPartInventorySystem.DTOs.Category;

namespace AutoPartInventorySystem.Services.Contracts
{
    public interface ICategoryService
    {
        Task<bool> AddAsync(AddCategoryDto dto);
        Task<bool> UpdateAsync(UpdateCategoryDto dto);
        Task<List<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto?> GetCategoryByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateImageAsync(UpdateCategoryImageDto dto);
    }
}
