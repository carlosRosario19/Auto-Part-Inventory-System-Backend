using AutoPartInventorySystem.DTOs;
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
    }
}
