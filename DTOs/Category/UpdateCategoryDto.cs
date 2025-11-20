using System.ComponentModel.DataAnnotations;

namespace AutoPartInventorySystem.DTOs.Category
{
    public class UpdateCategoryDto
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        // Optional replacement image
        public IFormFile? Image { get; set; }
    }
}
