using AutoPartInventorySystem.Models;

namespace AutoPartInventorySystem.Repositories.Contracts
{
    public interface IBrandRepository
    {
        Task<Brand?> GetByIdAsync(int id);
        Task<Brand?> GetByNameAsync(string name);
        Task<List<Brand>> GetAllAsync();
        Task AddAsync(Brand brand);
        Task UpdateAsync(Brand brand);
        Task DeleteAsync(Brand brand);
        Task<List<Brand>> GetByIdsAsync(List<int> ids);
    }
}
