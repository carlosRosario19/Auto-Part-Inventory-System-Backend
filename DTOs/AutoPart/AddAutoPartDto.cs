using System.ComponentModel.DataAnnotations;

namespace AutoPartInventorySystem.DTOs.AutoPart
{
    public class AddAutoPartDto
    {
        [Required]
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        [Required]
        public IFormFile Image { get; set; } = null!;
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public decimal Cost { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string? Location { get; set; }
        [Required]
        public List<int> BrandIds { get; set; } = new();
    }
}
