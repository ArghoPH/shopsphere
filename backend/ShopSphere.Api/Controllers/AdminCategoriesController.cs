using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopSphere.Api.Data;
using ShopSphere.Api.DTOs;
using ShopSphere.Api.Models;

namespace ShopSphere.Api.Controllers;

[ApiController]
[Route("api/admin/categories")]
[Authorize(Roles = "Admin,MasterAdmin")]
public class AdminCategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public AdminCategoriesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Slug,
                c.CreatedAt,
                c.ImageUrl,
                ProductCount = c.Products.Count()
            })
            .ToListAsync();

        return Ok(categories);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory(CreateCategoryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { message = "Category name is required" });
        }

        if (string.IsNullOrWhiteSpace(request.Slug))
        {
            return BadRequest(new { message = "Category slug is required" });
        }

        var slug = request.Slug.Trim().ToLower();

        var slugExists = await _context.Categories
            .AnyAsync(c => c.Slug == slug);

        if (slugExists)
        {
            return BadRequest(new { message = "Slug already exists" });
        }

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Slug = slug,
            ImageUrl = request.ImageUrl,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Category created successfully",
            categoryId = category.Id
        });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCategory(Guid id, UpdateCategoryRequest request)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return NotFound(new { message = "Category not found" });
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { message = "Category name is required" });
        }

        if (string.IsNullOrWhiteSpace(request.Slug))
        {
            return BadRequest(new { message = "Category slug is required" });
        }

        var slug = request.Slug.Trim().ToLower();

        var slugExists = await _context.Categories
            .AnyAsync(c => c.Slug == slug && c.Id != id);

        if (slugExists)
        {
            return BadRequest(new { message = "Slug already exists" });
        }

        category.Name = request.Name.Trim();
        category.Slug = slug;
        category.ImageUrl = request.ImageUrl;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Category updated successfully" });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return NotFound(new { message = "Category not found" });
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Category deleted successfully" });
    }
}