using AutoPartInventorySystem.DTOs;
using AutoPartInventorySystem.Services.Contracts;
using AutoPartInventorySystem.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoPartInventorySystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("add-staff")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddStaff([FromBody] AddUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // automatic validation errors
            }

            var success = await _userService.AddAsync(dto);
            if (!success)
            {
                return Conflict(new { message = "Email already exists." });
            }

            return CreatedAtAction(nameof(AddStaff), new { email = dto.Email }, new { message = "User created successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var token = await _userService.LoginAsync(dto);
            if (token == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            return Ok(new { token });
        }

        [HttpPut("update-staff")]
        [Authorize(Roles = "staff,admin")]
        public async Task<IActionResult> UpdateStaff([FromBody] UpdateUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.UpdateAsync(dto);

            if (result == UpdateUserResult.NotFound)
            {
                return NotFound(new { message = "User not found." });
            }

            if (result == UpdateUserResult.EmailAlreadyExists)
            {
                return Conflict(new { message = "Email already belongs to another user." });
            }

            return NoContent();
        }

        [HttpGet("get-users")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<PagedResult<UserDTO>>> GetAllUsers(int pageNumber = 1,int pageSize = 10)
        {
            var result = await _userService.GetAllUsersAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("get-user/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<UserDTO>> GetUser([FromRoute] int  id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
                return NotFound(new { message = "User not found." });

            return Ok(user);
        } 

        [HttpDelete("delete-user/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            var deleted = await _userService.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { message = "User not found" });

            return NoContent();
        }

        [HttpPatch("promote/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PromoteToAdmin([FromRoute] int id)
        {
            var success = await _userService.PromoteToAdminAsync(id);

            if (!success)
                return NotFound(new { message = "User not found." });

            return NoContent();
        }

    }
}
