using AutoPartInventorySystem.Data;
using AutoPartInventorySystem.Models;
using AutoPartInventorySystem.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace AutoPartInventorySystem.Repositories.Implementations
{
    public class BrandRepository : IBrandRepository
    {
        private readonly AutoPartInventoryDBContext _dbContext;

        public BrandRepository(AutoPartInventoryDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Brand brand)
        {
            await _dbContext.Brands.AddAsync(brand);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Brand brand)
        {
            _dbContext.Brands.Remove(brand);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Brand>> GetAllAsync()
        {
            return await _dbContext.Brands
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Brand?> GetByIdAsync(int id)
        {
            return await _dbContext.Brands
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BrandId == id);
        }

        public async Task<List<Brand>> GetByIdsAsync(List<int> ids)
        {
            return await _dbContext.Brands
                .Where(b => ids.Contains(b.BrandId))
                .ToListAsync();
        }

        public async Task<Brand?> GetByNameAsync(string name)
        {
            return await _dbContext.Brands
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Name == name);
        }

        public async Task UpdateAsync(Brand brand)
        {
            _dbContext.Brands.Update(brand);
            await _dbContext.SaveChangesAsync();
        }
    }
}
