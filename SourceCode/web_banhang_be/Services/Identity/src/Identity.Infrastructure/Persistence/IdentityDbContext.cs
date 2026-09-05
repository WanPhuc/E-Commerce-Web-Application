using Microsoft.EntityFrameworkCore;
using AuraMart.Identity.Domain;

namespace AuraMart.Identity.Infrastructure.Persistence;

// DbContext RIENG cua Identity service - chi so huu cac bang identity.
// Day la buoc "database per service" dau tien: du lieu nam trong DB
// AuraMart_Identity, tach khoi DB cua core app.
public class IdentityDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User - Role (n : 1)
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // User - Address (1 : n)
        modelBuilder.Entity<Address>()
            .HasOne(a => a.User)
            .WithMany(u => u.Addresses)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // RefreshToken: FK tu dinh nghia trong entity (UserId) neu co nav;
        // giu don gian: khong cau hinh them.
    }
}
