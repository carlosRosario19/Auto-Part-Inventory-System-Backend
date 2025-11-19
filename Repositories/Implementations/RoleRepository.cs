using AutoPartInventorySystem.Data;
using AutoPartInventorySystem.Models;
using AutoPartInventorySystem.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace AutoPartInventorySystem.Repositories.Implementations
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AutoPartInventoryDBContext _dbContext;

        public RoleRepository(AutoPartInventoryDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Role> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Role name cannot be null or empty.", nameof(name));

            var role = await _dbContext.Roles
                .FirstOrDefaultAsync(r => r.Name == name);

            if (role == null)
                throw new KeyNotFoundException($"Role '{name}' not found.");

            return role;
        }
    }
}
