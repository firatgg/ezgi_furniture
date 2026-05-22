namespace ezgi_mobilya.Core.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
        public bool IsActive { get; set; } = true;
        
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
