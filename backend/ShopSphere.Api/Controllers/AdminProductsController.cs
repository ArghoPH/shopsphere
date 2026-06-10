using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopSphere.Api.Data;
using ShopSphere.Api.DTOs;
using ShopSphere.Api.Models;

namespace ShopSphere.Api.Controllers;

[ApiController]
[Route("api/admin/products")]
[Authorize(Roles = "Admin,MasterAdmin")]
public class AdminProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AdminProductsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _context.Products
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new
            {
                p.Id,
                p.CategoryId,
                CategoryName = p.Category != null ? p.Category.Name : null,
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

    [HttpPost]
    public async Task<IActionResult> CreateProduct(CreateProductRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { message = "Product name is required" });
        }

        if (string.IsNullOrWhiteSpace(request.Slug))
        {
            return BadRequest(new { message = "Product slug is required" });
        }

        if (request.Price < 0)
        {
            return BadRequest(new { message = "Price cannot be negative" });
        }

        if (request.Stock < 0)
        {
            return BadRequest(new { message = "Stock cannot be negative" });
        }

        var slugExists = await _context.Products.AnyAsync(p => p.Slug == request.Slug);

        if (slugExists)
        {
            return BadRequest(new { message = "Slug already exists" });
        }

        if (request.CategoryId.HasValue)
        {
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == request.CategoryId.Value);

            if (!categoryExists)
            {
                return BadRequest(new { message = "Category not found" });
            }
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            CategoryId = request.CategoryId,
            Name = request.Name,
            Slug = request.Slug,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            ImageUrl = request.ImageUrl,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Product created successfully",
            productId = product.Id
        });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProduct(Guid id, UpdateProductRequest request)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound(new { message = "Product not found" });
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { message = "Product name is required" });
        }

        if (string.IsNullOrWhiteSpace(request.Slug))
        {
            return BadRequest(new { message = "Product slug is required" });
        }

        if (request.Price < 0)
        {
            return BadRequest(new { message = "Price cannot be negative" });
        }

        if (request.Stock < 0)
        {
            return BadRequest(new { message = "Stock cannot be negative" });
        }

        var slugExists = await _context.Products
            .AnyAsync(p => p.Slug == request.Slug && p.Id != id);

        if (slugExists)
        {
            return BadRequest(new { message = "Slug already exists" });
        }

        product.CategoryId = request.CategoryId;
        product.Name = request.Name;
        product.Slug = request.Slug;
        product.Description = request.Description;
        product.Price = request.Price;
        product.Stock = request.Stock;
        product.ImageUrl = request.ImageUrl;
        product.IsActive = request.IsActive;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Product updated successfully" });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeactivateProduct(Guid id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound(new { message = "Product not found" });
        }

        product.IsActive = false;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Product deactivated successfully" });
    }
}