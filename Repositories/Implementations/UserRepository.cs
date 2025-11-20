using AutoPartInventorySystem.Data;
using AutoPartInventorySystem.Models;
using AutoPartInventorySystem.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace AutoPartInventorySystem.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AutoPartInventoryDBContext _context;

        public UserRepository(AutoPartInventoryDBContext context)
        {
            _context = context;
        }
        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<(IEnumerable<User> Users, int TotalCount)> GetAllUsersPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Users
                .Include(u => u.Roles)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var users = await query
                .OrderBy(u => u.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalCount);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Roles)        // include roles for JWT claims
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
