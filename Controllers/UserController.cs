using AutoPartInventorySystem.DTOs;
using AutoPartInventorySystem.Services.Contracts;
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
    }
}
