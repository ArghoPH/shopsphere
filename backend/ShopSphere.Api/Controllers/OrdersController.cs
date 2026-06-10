using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopSphere.Api.Data;
using ShopSphere.Api.DTOs;
using ShopSphere.Api.Models;
using System.Security.Claims;

namespace ShopSphere.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrdersController(AppDbContext context)
    {
        _context = context;
    }

    private Guid? GetCurrentUserId()
    {
        var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdText, out var userId))
        {
            return null;
        }

        return userId;
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout(CheckoutRequest request)
    {
        var currentUserId = GetCurrentUserId();

        if (currentUserId == null)
        {
            return Unauthorized();
        }

        if (request.UserId != Guid.Empty && request.UserId != currentUserId.Value)
        {
            return Forbid();
        }

        if (string.IsNullOrWhiteSpace(request.FullName) ||
            string.IsNullOrWhiteSpace(request.Phone) ||
            string.IsNullOrWhiteSpace(request.Address))
        {
            return BadRequest(new { message = "Full name, phone and address are required" });
        }

        if (!request.CashOnDeliveryChecked)
        {
            return BadRequest(new { message = "Please select Cash on Delivery" });
        }

        if (!request.ConfirmOrder)
        {
            return BadRequest(new { message = "Please confirm your order" });
        }

        var cart = await _context.Carts
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == currentUserId.Value);

        if (cart == null || !cart.CartItems.Any())
        {
            return BadRequest(new { message = "Cart is empty" });
        }

        foreach (var item in cart.CartItems)
        {
            if (item.Product == null)
            {
                return BadRequest(new { message = "One or more products are unavailable" });
            }

            if (!item.Product.IsActive)
            {
                return BadRequest(new { message = $"{item.Product.Name} is currently inactive" });
            }

            if (item.Product.Stock < item.Quantity)
            {
                return BadRequest(new { message = $"Not enough stock for {item.Product.Name}" });
            }
        }

        var totalAmount = cart.CartItems.Sum(item => item.Price * item.Quantity);

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = currentUserId.Value,
            FullName = request.FullName.Trim(),
            Phone = request.Phone.Trim(),
            Address = request.Address.Trim(),
            TotalAmount = totalAmount,
            PaymentMethod = "CashOnDelivery",
            PaymentStatus = "Pending",
            OrderStatus = "Processing",
            CreatedAt = DateTimeOffset.UtcNow
        };

        _context.Orders.Add(order);

        foreach (var item in cart.CartItems)
        {
            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = item.ProductId,
                ProductName = item.Product!.Name,
                Quantity = item.Quantity,
                Price = item.Price,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _context.OrderItems.Add(orderItem);

            item.Product.Stock -= item.Quantity;
        }

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
        var currentUserId = GetCurrentUserId();

        if (currentUserId == null)
        {
            return Unauthorized();
        }

        if (userId != currentUserId.Value)
        {
            return Forbid();
        }

        var orders = await _context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == currentUserId.Value)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new
            {
                o.Id,
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
}