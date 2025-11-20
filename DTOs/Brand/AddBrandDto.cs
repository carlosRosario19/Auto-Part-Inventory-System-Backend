using System.ComponentModel.DataAnnotations;

namespace AutoPartInventorySystem.DTOs.Brand
{
    public class AddBrandDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        public IFormFile Image { get; set; } = null!;
    }
}
