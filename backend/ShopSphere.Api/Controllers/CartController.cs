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
public class CartController : ControllerBase
{
    private readonly AppDbContext _context;

    public CartController(AppDbContext context)
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

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetCart(Guid userId)
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

        var cart = await _context.Carts
            .AsNoTracking()
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            return Ok(new
            {
                cartId = (Guid?)null,
                userId,
                items = new List<object>(),
                total = 0
            });
        }

        var items = cart.CartItems.Select(ci => new
        {
            ci.Id,
            ci.ProductId,
            ProductName = ci.Product != null ? ci.Product.Name : "Unknown Product",
            ImageUrl = ci.Product != null ? ci.Product.ImageUrl : null,
            ci.Quantity,
            ci.Price,
            Subtotal = ci.Price * ci.Quantity
        }).ToList();

        return Ok(new
        {
            cartId = cart.Id,
            cart.UserId,
            items,
            total = items.Sum(i => i.Subtotal)
        });
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddToCart(AddToCartRequest request)
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

        if (request.Quantity <= 0)
        {
            return BadRequest(new { message = "Quantity must be greater than zero" });
        }

        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.IsActive);

        if (product == null)
        {
            return NotFound(new { message = "Product not found" });
        }

        if (product.Stock < request.Quantity)
        {
            return BadRequest(new { message = "Not enough stock available" });
        }

        var cart = await _context.Carts
            .FirstOrDefaultAsync(c => c.UserId == currentUserId.Value);

        if (cart == null)
        {
            cart = new Cart
            {
                Id = Guid.NewGuid(),
                UserId = currentUserId.Value,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        var existingItem = await _context.CartItems
            .FirstOrDefaultAsync(ci =>
                ci.CartId == cart.Id &&
                ci.ProductId == request.ProductId);

        if (existingItem != null)
        {
            var newQuantity = existingItem.Quantity + request.Quantity;

            if (product.Stock < newQuantity)
            {
                return BadRequest(new { message = "Not enough stock available" });
            }

            existingItem.Quantity = newQuantity;
        }
        else
        {
            var cartItem = new CartItem
            {
                Id = Guid.NewGuid(),
                CartId = cart.Id,
                ProductId = product.Id,
                Quantity = request.Quantity,
                Price = product.Price,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _context.CartItems.Add(cartItem);
        }

        await _context.SaveChangesAsync();

        return Ok(new { message = "Product added to cart successfully" });
    }

    [HttpPut("items/{cartItemId:guid}")]
    public async Task<IActionResult> UpdateCartItem(Guid cartItemId, UpdateCartItemRequest request)
    {
        var currentUserId = GetCurrentUserId();

        if (currentUserId == null)
        {
            return Unauthorized();
        }

        if (request.Quantity <= 0)
        {
            return BadRequest(new { message = "Quantity must be greater than zero" });
        }

        var cartItem = await _context.CartItems
            .Include(ci => ci.Cart)
            .Include(ci => ci.Product)
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId);

        if (cartItem == null)
        {
            return NotFound(new { message = "Cart item not found" });
        }

        if (cartItem.Cart == null || cartItem.Cart.UserId != currentUserId.Value)
        {
            return Forbid();
        }

        if (cartItem.Product != null && cartItem.Product.Stock < request.Quantity)
        {
            return BadRequest(new { message = "Not enough stock available" });
        }

        cartItem.Quantity = request.Quantity;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Cart item updated successfully" });
    }

    [HttpDelete("items/{cartItemId:guid}")]
    public async Task<IActionResult> RemoveCartItem(Guid cartItemId)
    {
        var currentUserId = GetCurrentUserId();

        if (currentUserId == null)
        {
            return Unauthorized();
        }

        var cartItem = await _context.CartItems
            .Include(ci => ci.Cart)
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId);

        if (cartItem == null)
        {
            return NotFound(new { message = "Cart item not found" });
        }

        if (cartItem.Cart == null || cartItem.Cart.UserId != currentUserId.Value)
        {
            return Forbid();
        }

        _context.CartItems.Remove(cartItem);

        await _context.SaveChangesAsync();

        return Ok(new { message = "Cart item removed successfully" });
    }
}