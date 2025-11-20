using AutoPartInventorySystem.DTOs;
using AutoPartInventorySystem.DTOs.User;
using AutoPartInventorySystem.Util;

namespace AutoPartInventorySystem.Services.Contracts
{
    public interface IUserService
    {
        Task<string?> LoginAsync(LoginDto loginDto);
        Task<bool> AddAsync(AddUserDto addUserDto);
        Task<UpdateUserResult> UpdateAsync(UpdateUserDto updateUserDto);
        Task<PagedResult<UserDTO>> GetAllUsersAsync(int pageNumber, int pageSize);
        Task<UserDTO?> GetUserByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<bool> PromoteToAdminAsync(int id);
    }
}
