using Microsoft.EntityFrameworkCore;
using ShopSphere.Api.Models;

namespace ShopSphere.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasColumnType("numeric(10,2)");

        modelBuilder.Entity<CartItem>()
            .Property(ci => ci.Price)
            .HasColumnType("numeric(10,2)");
    }
}