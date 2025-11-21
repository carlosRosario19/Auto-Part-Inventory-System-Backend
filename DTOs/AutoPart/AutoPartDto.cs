using AutoPartInventorySystem.DTOs.Brand;

namespace AutoPartInventorySystem.DTOs.AutoPart
{
    public class AutoPartDto
    {
        public int AutoPartId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public decimal Cost { get; set; }
        public decimal Price { get; set; }
        public string? Location { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<BrandDto> Brands { get; set; } = new();
    }
}
