namespace AutoPartInventorySystem.DTOs.AutoPart
{
    public class UpdateAutoPartDto
    {
        public int AutoPartId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
        public int CategoryId { get; set; }
        public decimal Cost { get; set; }
        public decimal Price { get; set; }
        public string? Location { get; set; }
        public List<int> BrandIds { get; set; } = new();
    }
}
