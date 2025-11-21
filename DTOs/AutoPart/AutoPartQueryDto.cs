namespace AutoPartInventorySystem.DTOs.AutoPart
{
    public class AutoPartQueryDto
    {
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
