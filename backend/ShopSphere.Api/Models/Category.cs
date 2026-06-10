namespace ShopSphere.Api.Models;

public class Category
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }

    public string? ImageUrl { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();


}
