using AutoPartInventorySystem.DTOs.AutoPart;
using AutoPartInventorySystem.Util;

namespace AutoPartInventorySystem.Services.Contracts
{
    public interface IAutoPartService
    {
        Task<bool> AddAsync(AddAutoPartDto dto);
        Task<bool> DeleteAsync(int id);
        Task<PagedResult<AutoPartDto>> GetAllAsync(AutoPartQueryDto query);
        Task<AutoPartDto?> GetAutoPartByIdAsync(int id);
        Task<bool> UpdateAsync(UpdateAutoPartDto dto);
        Task<LinkVehicleResult> LinkVehicleAsync(LinkPartWithVehicleDto dto);
    }
}
