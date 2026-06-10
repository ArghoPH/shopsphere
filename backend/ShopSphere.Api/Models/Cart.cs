namespace ShopSphere.Api.Models;

public class Cart
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}