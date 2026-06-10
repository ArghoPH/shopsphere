namespace ShopSphere.Api.DTOs;

public class AddToCartRequest
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; } = 1;
}