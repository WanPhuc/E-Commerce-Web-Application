using Microsoft.EntityFrameworkCore;
using AuraMart.Ordering.Domain;
using BuildingBlocks.Outbox;

namespace AuraMart.Ordering.Infrastructure.Persistence;

public class OrderingDbContext : DbContext
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<OrderShippingAddress> ShippingAddresses => Set<OrderShippingAddress>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<ProcessedEvent> ProcessedEvents => Set<ProcessedEvent>();

    public OrderingDbContext(DbContextOptions<OrderingDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>().OwnsOne(o => o.ShippingAddress, sa =>
        {
            sa.Property(x => x.RecipientName).HasColumnName("ShipRecipientName");
            sa.Property(x => x.PhoneNumber).HasColumnName("ShipPhoneNumber");
            sa.Property(x => x.AddressLine).HasColumnName("ShipAddressLine");
            sa.Property(x => x.Ward).HasColumnName("ShipWard");
            sa.Property(x => x.District).HasColumnName("ShipDistrict");
            sa.Property(x => x.City).HasColumnName("ShipCity");
        });

        modelBuilder.Entity<OutboxMessage>().ToTable("OutboxMessages");
        modelBuilder.Entity<ProcessedEvent>().ToTable("ProcessedEvents");
    }
}
