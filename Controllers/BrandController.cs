using AutoPartInventorySystem.DTOs.Brand;
using AutoPartInventorySystem.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoPartInventorySystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        // POST: api/brand
        [HttpPost]
        [Authorize(Roles = "staff,admin")]
        public async Task<IActionResult> AddBrand([FromForm] AddBrandDto dto)
        {
            var success = await _brandService.AddAsync(dto);
            if (!success)
                return BadRequest("Brand name already exists or image upload failed.");

            return Ok("Brand created successfully.");
        }

        // GET: api/brand
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var brands = await _brandService.GetAllBrandsAsync();
            return Ok(brands);
        }

        // GET: api/brand/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var brand = await _brandService.GetBrandByIdAsync(id);
            if (brand == null)
                return NotFound("Brand not found.");

            return Ok(brand);
        }

        // PUT: api/brand
        [HttpPut]
        [Authorize(Roles = "staff,admin")]
        public async Task<IActionResult> UpdateBrand([FromForm] UpdateBrandDto dto)
        {
            var success = await _brandService.UpdateAsync(dto);
            if (!success)
                return NotFound("Brand not found or update failed.");

            return Ok("Brand updated successfully.");
        }

        // PATCH: api/brand/image
        [HttpPatch("image")]
        [Authorize(Roles = "staff,admin")]
        public async Task<IActionResult> UpdateBrandImage([FromForm] UpdateBrandImageDto dto)
        {
            var success = await _brandService.UpdateImageAsync(dto);
            if (!success)
                return NotFound("Brand not found or image update failed.");

            return Ok("Brand image updated successfully.");
        }

        // DELETE: api/brand/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "staff,admin")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var success = await _brandService.DeleteAsync(id);
            if (!success)
                return NotFound("Brand not found.");

            return Ok("Brand deleted successfully.");
        }

    }
}
