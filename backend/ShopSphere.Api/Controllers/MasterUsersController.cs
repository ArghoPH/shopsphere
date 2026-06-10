using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopSphere.Api.Data;
using ShopSphere.Api.DTOs;
using ShopSphere.Api.Models;

namespace ShopSphere.Api.Controllers;

[ApiController]
[Route("api/master/users")]
[Authorize(Roles = "MasterAdmin")]
public class MasterUsersController : ControllerBase
{
    private readonly AppDbContext _context;

    private static readonly string[] AllowedRoles =
    {
        "MasterAdmin",
        "Admin",
        "User"
    };

    public MasterUsersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _context.AppUsers
            .AsNoTracking()
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                u.Role,
                u.IsActive,
                u.CreatedAt
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
    {
        if (!AllowedRoles.Contains(request.Role))
        {
            return BadRequest(new { message = "Invalid role" });
        }

        if (string.IsNullOrWhiteSpace(request.FullName) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Full name, email and password are required" });
        }

        var email = request.Email.Trim().ToLower();

        var emailExists = await _context.AppUsers.AnyAsync(u => u.Email == email);

        if (emailExists)
        {
            return BadRequest(new { message = "Email already exists" });
        }

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _context.AppUsers.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "User created successfully",
            userId = user.Id,
            role = user.Role
        });
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] bool isActive)
    {
        var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        user.IsActive = isActive;

        await _context.SaveChangesAsync();

        return Ok(new { message = "User status updated successfully" });
    }
}