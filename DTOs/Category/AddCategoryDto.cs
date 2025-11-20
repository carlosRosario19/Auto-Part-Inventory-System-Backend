using System.ComponentModel.DataAnnotations;

namespace AutoPartInventorySystem.DTOs.Category
{
    public class AddCategoryDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        // File uploaded by client
        [Required]
        public IFormFile Image { get; set; } = null!;
    }
}
