namespace ShopSphere.Api.DTOs;

public class CheckoutRequest
{
    public Guid UserId { get; set; }

    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    public bool CashOnDeliveryChecked { get; set; }
    public bool ConfirmOrder { get; set; }
}