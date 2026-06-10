namespace ShopSphere.Api.Models;

public class Order
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public string PaymentMethod { get; set; } = "CashOnDelivery";
    public string PaymentStatus { get; set; } = "Pending";
    public string OrderStatus { get; set; } = "Processing";

    public DateTimeOffset CreatedAt { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}