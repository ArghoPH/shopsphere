namespace ShopSphere.Api.Models;

public class Product
{
    public Guid Id { get; set; }
    public Guid? CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }

    public decimal Price { get; set; }
    public int Stock { get; set; }

    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public Category? Category { get; set; }
}