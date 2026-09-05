using Microsoft.EntityFrameworkCore;
using BuildingBlocks.Outbox;

namespace AuraMart.Payment.Infrastructure.Persistence;

public class PaymentDbContext : DbContext
{
    public DbSet<AuraMart.Payment.Domain.Payment> Payments { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<AuraMart.Payment.Domain.Payment>().ToTable("Payments");
        modelBuilder.Entity<OutboxMessage>().ToTable("OutboxMessages");
    }
}
