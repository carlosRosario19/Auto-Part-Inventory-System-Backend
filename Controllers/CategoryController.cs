using AutoPartInventorySystem.DTOs.Category;
using AutoPartInventorySystem.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoPartInventorySystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // POST: api/category
        [HttpPost]
        [Authorize(Roles = "staff,admin")]
        public async Task<IActionResult> AddCategory([FromForm] AddCategoryDto dto)
        {
            var success = await _categoryService.AddAsync(dto);
            if (!success)
                return BadRequest("Category name already exists or image upload failed.");

            return Ok("Category created successfully.");
        }

        // GET: api/category
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        // GET: api/category/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound("Category not found.");

            return Ok(category);
        }

        // PUT: api/category
        [HttpPut]
        [Authorize(Roles = "staff,admin")]
        public async Task<IActionResult> UpdateCategory([FromForm] UpdateCategoryDto dto)
        {
            var success = await _categoryService.UpdateAsync(dto);
            if (!success)
                return NotFound("Category not found or update failed.");

            return Ok("Category updated successfully.");
        }

        // DELETE: api/category/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "staff,admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var success = await _categoryService.DeleteAsync(id);
            if (!success)
                return NotFound("Category not found.");

            return Ok("Category deleted successfully.");
        }

        // PATCH: api/category
        [HttpPatch("update-image")]
        [Authorize(Roles = "staff,admin")]
        public async Task<IActionResult> UpdateImage([FromForm] UpdateCategoryImageDto dto)
        {
            var success = await _categoryService.UpdateImageAsync(dto);

            if (!success)
                return NotFound("Category not found or image update failed.");

            return Ok("Category image updated successfully.");
        }
    }
}
