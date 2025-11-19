using AutoPartInventorySystem.Models;

namespace AutoPartInventorySystem.Repositories.Contracts
{
    public interface IRoleRepository
    {
        Task<Role> GetByNameAsync(String name);
    }
}
