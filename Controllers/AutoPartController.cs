using AutoPartInventorySystem.DTOs.AutoPart;
using AutoPartInventorySystem.Services.Contracts;
using AutoPartInventorySystem.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoPartInventorySystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutoPartController : ControllerBase
    {
        private readonly IAutoPartService _autoPartService;

        public AutoPartController(IAutoPartService autoPartService)
        {
            _autoPartService = autoPartService;
        }

        // ------------------------------------------------------------
        // CREATE
        // ------------------------------------------------------------
        [HttpPost]
        [Authorize(Roles = "admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Add([FromForm] AddAutoPartDto dto)
        {
            var result = await _autoPartService.AddAsync(dto);
            if (!result)
                return BadRequest("Could not create AutoPart. Invalid data or upload failure.");

            return StatusCode(201); // Created
        }

        // ------------------------------------------------------------
        // GET ALL (FILTERS + PAGINATION)
        // ------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] AutoPartQueryDto query)
        {
            var result = await _autoPartService.GetAllAsync(query);
            return Ok(result);
        }

        // ------------------------------------------------------------
        // GET BY ID
        // ------------------------------------------------------------
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var autoPart = await _autoPartService.GetAutoPartByIdAsync(id);

            if (autoPart == null)
                return NotFound();

            return Ok(autoPart);
        }

        // ------------------------------------------------------------
        // UPDATE
        // ------------------------------------------------------------
        [HttpPut]
        [Authorize(Roles = "admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromForm] UpdateAutoPartDto dto)
        {
            var updated = await _autoPartService.UpdateAsync(dto);
            if (!updated)
                return BadRequest("Update failed. Invalid data or image upload error.");

            return NoContent();
        }

        // ------------------------------------------------------------
        // DELETE
        // ------------------------------------------------------------
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _autoPartService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpPatch("link-vehicle")]
        public async Task<IActionResult> LinkVehicle([FromBody] LinkPartWithVehicleDto dto)
        {
            var result = await _autoPartService.LinkVehicleAsync(dto);

            return result switch
            {
                LinkVehicleResult.Success => Ok("Vehicle linked successfully."),
                LinkVehicleResult.AutoPartNotFound => NotFound("Auto-part not found."),
                LinkVehicleResult.BrandNotFound => NotFound("Brand not found."),
                LinkVehicleResult.InvalidYearRange => BadRequest("Invalid year range."),
                LinkVehicleResult.AlreadyLinked => Conflict("This vehicle is already linked."),
                _ => StatusCode(500, "An unexpected error occurred.")
            };
        }
    }
}
