using AutoPartInventorySystem.Models;

namespace AutoPartInventorySystem.Repositories.Contracts
{
    public interface IVehicleRepository
    {
        Task AddAsync(Vehicle vehicle);
        Task<Vehicle?> FindExistingVehicleAsync(int brandId, string model, int startYear, int? endYear);
    }
}
