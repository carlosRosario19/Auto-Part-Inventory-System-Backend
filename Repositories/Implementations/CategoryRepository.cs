using AutoPartInventorySystem.Data;
using AutoPartInventorySystem.Models;
using AutoPartInventorySystem.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace AutoPartInventorySystem.Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AutoPartInventoryDBContext _dbContext;

        public CategoryRepository(AutoPartInventoryDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(Category category)
        {
            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Category category)
        {
            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _dbContext.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _dbContext.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        public async Task<Category?> GetByNameAsync(string name)
        {
            return await _dbContext.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
        }

        public async Task UpdateAsync(Category category)
        {
            _dbContext.Categories.Update(category);
            await _dbContext.SaveChangesAsync();
        }
    }
}
