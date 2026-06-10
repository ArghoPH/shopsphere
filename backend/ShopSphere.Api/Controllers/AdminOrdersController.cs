using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopSphere.Api.Data;
using ShopSphere.Api.DTOs;

namespace ShopSphere.Api.Controllers;

[ApiController]
[Route("api/admin/orders")]
[Authorize(Roles = "Admin,MasterAdmin")]
public class AdminOrdersController : ControllerBase
{
    private readonly AppDbContext _context;

    private static readonly string[] AllowedStatuses =
    {
        "Processing",
        "Confirmed",
        "Shipped",
        "Delivered",
        "Cancelled"
    };

    public AdminOrdersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        var orders = await _context.Orders
            .AsNoTracking()
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new
            {
                o.Id,
                o.UserId,
                o.FullName,
                o.Phone,
                o.Address,
                o.TotalAmount,
                o.PaymentMethod,
                o.PaymentStatus,
                o.OrderStatus,
                o.CreatedAt,
                Items = o.OrderItems.Select(oi => new
                {
                    oi.ProductId,
                    oi.ProductName,
                    oi.Quantity,
                    oi.Price,
                    Subtotal = oi.Price * oi.Quantity
                }).ToList()
            })
            .ToListAsync();

        return Ok(orders);
    }

    [HttpPut("{orderId:guid}/status")]
    public async Task<IActionResult> UpdateOrderStatus(Guid orderId, UpdateOrderStatusRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.OrderStatus))
        {
            return BadRequest(new { message = "Order status is required" });
        }

        var normalizedStatus = AllowedStatuses
            .FirstOrDefault(status =>
                status.Equals(request.OrderStatus, StringComparison.OrdinalIgnoreCase));

        if (normalizedStatus == null)
        {
            return BadRequest(new
            {
                message = "Invalid order status",
                allowedStatuses = AllowedStatuses
            });
        }

        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
        {
            return NotFound(new { message = "Order not found" });
        }

        order.OrderStatus = normalizedStatus;

        if (normalizedStatus == "Delivered")
        {
            order.PaymentStatus = "Paid";
        }

        if (normalizedStatus == "Cancelled")
        {
            order.PaymentStatus = "Cancelled";
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Order status updated successfully",
            orderId = order.Id,
            orderStatus = order.OrderStatus,
            paymentStatus = order.PaymentStatus
        });
    }
}