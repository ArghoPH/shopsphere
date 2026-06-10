using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopSphere.Api.Data;
using ShopSphere.Api.DTOs;
using ShopSphere.Api.Models;

namespace ShopSphere.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly AppDbContext _context;

    public CartController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetCart(Guid userId)
    {
        var cart = await _context.Carts
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .Select(c => new
            {
                c.Id,
                c.UserId,
                Items = c.CartItems.Select(ci => new
                {
                    ci.Id,
                    ci.ProductId,
                    ProductName = ci.Product != null ? ci.Product.Name : "Unknown Product",
                    ImageUrl = ci.Product != null ? ci.Product.ImageUrl : null,
                    ci.Quantity,
                    ci.Price,
                    Subtotal = ci.Price * ci.Quantity
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (cart == null)
        {
            return Ok(new
            {
                userId,
                items = Array.Empty<object>(),
                totalAmount = 0
            });
        }

        return Ok(new
        {
            cart.Id,
            cart.UserId,
            cart.Items,
            TotalAmount = cart.Items.Sum(i => i.Subtotal)
        });
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddToCart(AddToCartRequest request)
    {
        if (request.UserId == Guid.Empty)
        {
            return BadRequest(new { message = "UserId is required" });
        }

        if (request.ProductId == Guid.Empty)
        {
            return BadRequest(new { message = "ProductId is required" });
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
            .FirstOrDefaultAsync(c => c.UserId == request.UserId);

        if (cart == null)
        {
            cart = new Cart
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        var existingItem = await _context.CartItems
            .FirstOrDefaultAsync(ci =>
                ci.CartId == cart.Id &&
                ci.ProductId == request.ProductId
            );

        if (existingItem != null)
        {
            var newQuantity = existingItem.Quantity + request.Quantity;

            if (product.Stock < newQuantity)
            {
                return BadRequest(new { message = "Not enough stock available" });
            }

            existingItem.Quantity = newQuantity;
            existingItem.Price = product.Price;
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

        return Ok(new { message = "Product added to cart" });
    }

    [HttpPut("items/{cartItemId:guid}")]
    public async Task<IActionResult> UpdateCartItem(Guid cartItemId, UpdateCartItemRequest request)
    {
        if (request.Quantity <= 0)
        {
            return BadRequest(new { message = "Quantity must be greater than zero" });
        }

        var cartItem = await _context.CartItems
            .Include(ci => ci.Product)
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId);

        if (cartItem == null)
        {
            return NotFound(new { message = "Cart item not found" });
        }

        if (cartItem.Product != null && cartItem.Product.Stock < request.Quantity)
        {
            return BadRequest(new { message = "Not enough stock available" });
        }

        cartItem.Quantity = request.Quantity;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Cart item updated" });
    }

    [HttpDelete("items/{cartItemId:guid}")]
    public async Task<IActionResult> RemoveCartItem(Guid cartItemId)
    {
        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId);

        if (cartItem == null)
        {
            return NotFound(new { message = "Cart item not found" });
        }

        _context.CartItems.Remove(cartItem);

        await _context.SaveChangesAsync();

        return Ok(new { message = "Cart item removed" });
    }
}