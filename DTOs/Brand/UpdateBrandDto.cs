using System.ComponentModel.DataAnnotations;

namespace AutoPartInventorySystem.DTOs.Brand
{
    public class UpdateBrandDto
    {
        [Required]
        public int BrandId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        public IFormFile? Image { get; set; }
    }
}
