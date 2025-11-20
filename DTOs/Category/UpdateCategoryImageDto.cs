using System.ComponentModel.DataAnnotations;

namespace AutoPartInventorySystem.DTOs.Category
{
    public class UpdateCategoryImageDto
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        public IFormFile Image { get; set; } = null!;
    }
}
