using AutoPartInventorySystem.DTOs.AutoPart;
using AutoPartInventorySystem.Models;
using AutoPartInventorySystem.Util;

namespace AutoPartInventorySystem.Repositories.Contracts
{
    public interface IAutoPartRepository
    {
        Task AddAsync(AutoPart autoPart);
        Task UpdateAsync(AutoPart autoPart);
        Task DeleteAsync(AutoPart autoPart);

        Task<AutoPart?> GetByIdAsync(int id);

        Task<PagedResult<AutoPart>> GetAllAsync(AutoPartQueryDto query);
    }
}
