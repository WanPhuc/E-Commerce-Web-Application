using Microsoft.EntityFrameworkCore;
using BuildingBlocks.Outbox;

namespace AuraMart.Catalog.Infrastructure.Persistence;

// DbContext RIENG cua Catalog service (database catalog_db).
public class CatalogDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<ProductReview> ProductReviews { get; set; }
    public DbSet<ProcessedEvent> ProcessedEvents { get; set; }

    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Category tu tham chieu (cha - con) - noi bo Catalog
        modelBuilder.Entity<Category>()
            .HasOne(c => c.Parent)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Product - ProductReview (1 : n)
        modelBuilder.Entity<ProductReview>()
            .HasOne(p => p.Product)
            .WithMany(pr => pr.Reviews)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProcessedEvent>().ToTable("ProcessedEvents");
    }
}
