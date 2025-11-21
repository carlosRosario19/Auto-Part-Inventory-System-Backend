using AutoPartInventorySystem.Data;
using AutoPartInventorySystem.Models;
using AutoPartInventorySystem.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace AutoPartInventorySystem.Repositories.Implementations
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly AutoPartInventoryDBContext _db;

        public VehicleRepository(AutoPartInventoryDBContext db)
        {
            _db = db;
        }

        // --------------------------------------------------------
        // ADD NEW VEHICLE
        // --------------------------------------------------------
        public async Task AddAsync(Vehicle vehicle)
        {
            _db.Vehicles.Add(vehicle);
            await _db.SaveChangesAsync();
        }

        // --------------------------------------------------------
        // FIND EXISTING VEHICLE BY BRAND + MODEL + YEARS
        // --------------------------------------------------------
        public async Task<Vehicle?> FindExistingVehicleAsync(
            int brandId,
            string model,
            int startYear,
            int? endYear)
        {
            return await _db.Vehicles
                .FirstOrDefaultAsync(v =>
                    v.BrandId == brandId &&
                    v.Model == model &&
                    v.StartYear == startYear &&
                    v.EndYear == endYear
                );
        }
    }
}
