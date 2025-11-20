using System.ComponentModel.DataAnnotations;

namespace AutoPartInventorySystem.DTOs.Brand
{
    public class UpdateBrandImageDto
    {
        [Required]
        public int BrandId { get; set; }

        [Required]
        public IFormFile Image { get; set; } = null!;
    }
}
