using Microsoft.EntityFrameworkCore;
using AuraMart.Seller.Domain;

namespace AuraMart.Seller.Infrastructure.Persistence;

public class SellerDbContext : DbContext
{
    public DbSet<Domain.Seller> Sellers => Set<Domain.Seller>();
    public DbSet<SellerApplication> SellerApplications => Set<SellerApplication>();

    public SellerDbContext(DbContextOptions<SellerDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Domain.Seller>().ToTable("Sellers");
        modelBuilder.Entity<SellerApplication>().ToTable("SellerApplications");
    }
}
