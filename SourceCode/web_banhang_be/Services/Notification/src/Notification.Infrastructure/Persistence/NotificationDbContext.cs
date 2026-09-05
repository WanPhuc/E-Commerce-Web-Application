using Microsoft.EntityFrameworkCore;
using AuraMart.Notification.Domain;

namespace AuraMart.Notification.Infrastructure.Persistence;

public class NotificationDbContext : DbContext
{
    public DbSet<Domain.Notification> Notifications => Set<Domain.Notification>();

    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Domain.Notification>().ToTable("Notifications");
    }
}
