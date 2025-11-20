namespace AutoPartInventorySystem.DTOs.Category
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string? ImageUrl { get; set; }
    }
}
