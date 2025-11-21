using AutoPartInventorySystem.Data;
using AutoPartInventorySystem.DTOs.AutoPart;
using AutoPartInventorySystem.Models;
using AutoPartInventorySystem.Repositories.Contracts;
using AutoPartInventorySystem.Util;
using Microsoft.EntityFrameworkCore;

namespace AutoPartInventorySystem.Repositories.Implementations
{
    public class AutoPartRepository : IAutoPartRepository
    {
        private readonly AutoPartInventoryDBContext _dbContext;

        public AutoPartRepository(AutoPartInventoryDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        // ------------------------------------------------------------
        // ADD
        // ------------------------------------------------------------
        public async Task AddAsync(AutoPart autoPart)
        {
            _dbContext.AutoParts.Add(autoPart);
            await _dbContext.SaveChangesAsync();
        }

        // ------------------------------------------------------------
        // DELETE
        // ------------------------------------------------------------
        public async Task DeleteAsync(AutoPart autoPart)
        {
            _dbContext.AutoParts.Remove(autoPart);
            await _dbContext.SaveChangesAsync();
        }

        // ------------------------------------------------------------
        // GET ALL WITH FILTERS + PAGINATION
        // ------------------------------------------------------------
        public async Task<PagedResult<AutoPart>> GetAllAsync(AutoPartQueryDto query)
        {
            var partsQuery = _dbContext.AutoParts
                .Include(ap => ap.Category)
                .Include(ap => ap.Brands)
                .Include(ap => ap.Vehicles)
                .AsQueryable();

            // --- FILTERS ---
            if (!string.IsNullOrWhiteSpace(query.Name))
                partsQuery = partsQuery.Where(ap => ap.Name.Contains(query.Name));

            if (!string.IsNullOrWhiteSpace(query.Description))
                partsQuery = partsQuery.Where(ap => ap.Description!.Contains(query.Description));

            if (query.CategoryId.HasValue)
                partsQuery = partsQuery.Where(ap => ap.CategoryId == query.CategoryId.Value);

            if (query.BrandId.HasValue)
            {
                // AutoPart has many brands → filter by relationship
                partsQuery = partsQuery.Where(ap =>
                    ap.Brands.Any(b => b.BrandId == query.BrandId.Value));
            }

            // --- PAGINATION ---
            int pageNumber = query.PageNumber <= 0 ? 1 : query.PageNumber;
            int pageSize = query.PageSize <= 0 ? 10 : query.PageSize;

            int totalCount = await partsQuery.CountAsync();

            var items = await partsQuery
                .OrderBy(ap => ap.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Return raw list; AutoPartService maps → AutoPartDto
            return new PagedResult<AutoPart>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        // ------------------------------------------------------------
        // GET BY ID (with related data)
        // ------------------------------------------------------------
        public async Task<AutoPart?> GetByIdAsync(int id)
        {
            return await _dbContext.AutoParts
                .Include(ap => ap.Category)
                .Include(ap => ap.Brands)
                .Include(ap => ap.Vehicles)
                .FirstOrDefaultAsync(ap => ap.AutoPartId == id);
        }

        // ------------------------------------------------------------
        // UPDATE
        // ------------------------------------------------------------
        public async Task UpdateAsync(AutoPart autoPart)
        {
            _dbContext.AutoParts.Update(autoPart);
            await _dbContext.SaveChangesAsync();
        }
    }
}
