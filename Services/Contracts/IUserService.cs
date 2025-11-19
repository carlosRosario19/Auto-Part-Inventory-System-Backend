using AutoPartInventorySystem.DTOs;

namespace AutoPartInventorySystem.Services.Contracts
{
    public interface IUserService
    {
        Task<string?> LoginAsync(LoginDto loginDto);
        Task<bool> AddAsync(AddUserDto addUserDto);
    }
}
