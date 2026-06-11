using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopSphere.Api.Data;

namespace ShopSphere.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] string? categorySlug)
    {
        var query = _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(categorySlug))
        {
            query = query.Where(p =>
                p.Category != null &&
                p.Category.Slug == categorySlug);
        }

        var products = await query
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new
            {
                p.Id,
                p.CategoryId,
                CategoryName = p.Category != null ? p.Category.Name : null,
                CategorySlug = p.Category != null ? p.Category.Slug : null,
                p.Name,
                p.Slug,
                p.Description,
                p.Price,
                p.Stock,
                p.ImageUrl,
                p.IsActive,
                p.CreatedAt
            })
            .ToListAsync();

        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Where(p => p.Id == id && p.IsActive)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Slug,
                p.Description,
                p.Price,
                p.Stock,
                p.ImageUrl,
                CategoryName = p.Category != null ? p.Category.Name : null
            })
            .FirstOrDefaultAsync();

        if (product == null)
        {
            return NotFound(new { message = "Product not found" });
        }

        return Ok(product);
    }

    [HttpGet("trending")]
    public async Task<IActionResult> GetTrendingProducts()
    {
        var products = await _context.Products
            .AsNoTracking()
            .Where(p => p.IsActive)
            .Select(p => new
            {
                p.Id,
                p.CategoryId,
                CategoryName = p.Category != null ? p.Category.Name : null,
                CategorySlug = p.Category != null ? p.Category.Slug : null,
                p.Name,
                p.Slug,
                p.Description,
                p.Price,
                p.Stock,
                p.ImageUrl,
                p.IsActive,
                p.CreatedAt,
                SoldQuantity = _context.OrderItems
                    .Where(oi => oi.ProductId == p.Id)
                    .Sum(oi => (int?)oi.Quantity) ?? 0
            })
            .OrderByDescending(p => p.SoldQuantity)
            .ThenByDescending(p => p.CreatedAt)
            .Take(20)
            .ToListAsync();

        return Ok(products);
    }

}