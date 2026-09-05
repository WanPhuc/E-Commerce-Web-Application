using Microsoft.EntityFrameworkCore;
using AuraMart.Cart.Domain;

namespace AuraMart.Cart.Infrastructure.Persistence;

public class CartDbContext : DbContext
{
    public DbSet<Domain.Cart> Carts => Set<Domain.Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();

    public CartDbContext(DbContextOptions<CartDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Domain.Cart>().ToTable("Carts");
        modelBuilder.Entity<CartItem>().ToTable("CartItems");
    }
}
