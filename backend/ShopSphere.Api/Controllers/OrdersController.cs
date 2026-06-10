using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopSphere.Api.Data;
using ShopSphere.Api.DTOs;
using ShopSphere.Api.Models;

namespace ShopSphere.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrdersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout(CheckoutRequest request)
    {
        if (request.UserId == Guid.Empty)
        {
            return BadRequest(new { message = "UserId is required" });
        }

        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            return BadRequest(new { message = "Full name is required" });
        }

        if (string.IsNullOrWhiteSpace(request.Phone))
        {
            return BadRequest(new { message = "Phone number is required" });
        }

        if (string.IsNullOrWhiteSpace(request.Address))
        {
            return BadRequest(new { message = "Address is required" });
        }

        if (!request.CashOnDeliveryChecked)
        {
            return BadRequest(new { message = "Please confirm Cash on Delivery" });
        }

        if (!request.ConfirmOrder)
        {
            return BadRequest(new { message = "Order confirmation is required" });
        }

        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == request.UserId);

        if (cart == null || !cart.CartItems.Any())
        {
            return BadRequest(new { message = "Cart is empty" });
        }

        foreach (var item in cart.CartItems)
        {
            if (item.Product == null || !item.Product.IsActive)
            {
                return BadRequest(new { message = "One or more products are unavailable" });
            }

            if (item.Product.Stock < item.Quantity)
            {
                return BadRequest(new
                {
                    message = $"Not enough stock for {item.Product.Name}"
                });
            }
        }

        var totalAmount = cart.CartItems.Sum(item => item.Price * item.Quantity);

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            FullName = request.FullName,
            Phone = request.Phone,
            Address = request.Address,
            TotalAmount = totalAmount,
            PaymentMethod = "CashOnDelivery",
            PaymentStatus = "Pending",
            OrderStatus = "Processing",
            CreatedAt = DateTimeOffset.UtcNow
        };

        foreach (var item in cart.CartItems)
        {
            order.OrderItems.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = item.ProductId,
                ProductName = item.Product?.Name ?? "Unknown Product",
                Quantity = item.Quantity,
                Price = item.Price,
                CreatedAt = DateTimeOffset.UtcNow
            });

            if (item.Product != null)
            {
                item.Product.Stock -= item.Quantity;
            }
        }

        _context.Orders.Add(order);
        _context.CartItems.RemoveRange(cart.CartItems);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Order placed successfully",
            orderId = order.Id,
            totalAmount = order.TotalAmount,
            paymentMethod = order.PaymentMethod,
            paymentStatus = order.PaymentStatus,
            orderStatus = order.OrderStatus
        });
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetUserOrders(Guid userId)
    {
        var orders = await _context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new
            {
                o.Id,
                o.TotalAmount,
                o.PaymentMethod,
                o.PaymentStatus,
                o.OrderStatus,
                o.CreatedAt,
                Items = o.OrderItems.Select(oi => new
                {
                    oi.ProductName,
                    oi.Quantity,
                    oi.Price,
                    Subtotal = oi.Price * oi.Quantity
                }).ToList()
            })
            .ToListAsync();

        return Ok(orders);
    }
}