using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;

namespace WebBanHang.Data;
public partial class AppDbContext : DbContext
{
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<ProductReview> ProductReviews { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Seller> Sellers { get; set; }
    public DbSet<SellerApplication> SellerApplications { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)   : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ===== Category tự tham chiếu (cha - con) =====
        modelBuilder.Entity<Category>()
            .HasOne(c => c.Parent)
            .WithMany(c => c.SubCategories)          // đúng theo model bạn
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Restrict);      // KHÔNG cho xóa cha nếu còn con

        // ===== User - Role (n : 1) =====
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);      // xóa Role không xoá User

        // ===== User - Seller (1 : 1) =====
        modelBuilder.Entity<Seller>()
            .HasOne(s => s.User)
            .WithOne(u => u.Seller)
            .HasForeignKey<Seller>(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);      // tránh vòng lặp xóa

        // ===== User - Address (1 : n) =====
        modelBuilder.Entity<Address>()
            .HasOne(a => a.User)
            .WithMany(u => u.Addresses)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);      // KHÔNG cascade để tránh multiple path

        // ===== User - Orders (1 : n) =====
        modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);      // ❗ quan trọng: tắt CASCADE ở đây

        // ===== Order - Address (n : 1) =====
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Address)
            .WithMany()                              // hoặc .WithMany(a => a.Orders) nếu bạn có navigation
            .HasForeignKey(o => o.AddressId)
            .OnDelete(DeleteBehavior.Restrict);      // ❗ cũng tắt CASCADE

        // ===== Order - Payment (1 : 1) =====
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Payment)
            .WithOne(p => p.Order)
            .HasForeignKey<Payment>(p => p.OrderId)
            .OnDelete(DeleteBehavior.Cascade);       // xóa Order thì xóa Payment là hợp lý

        // ===== SellerApplication - User (1 : n) =====
        modelBuilder.Entity<SellerApplication>()
            .HasOne(sa => sa.User)
            .WithMany(u => u.SellerApplications)
            .HasForeignKey(sa => sa.UserId)
            .OnDelete(DeleteBehavior.Restrict);      // không cascade (tránh thêm path)

        // ===== Seller - Orders (1 : n) =====
        modelBuilder.Entity<Order>()
            .HasOne(o=>o.Seller)
            .WithMany(s=>s.Orders)
            .HasForeignKey(o=>o.SellerId)
            .OnDelete(DeleteBehavior.Restrict);    

        // ===== Seller - Address (1 : 1) =====
        modelBuilder.Entity<Address>()
            .HasOne(a=>a.Seller)
            .WithOne(s=>s.Address)
            .HasForeignKey<Seller>(s=>s.AddressId)
            .OnDelete(DeleteBehavior.Restrict);  

        //=====Product - ProductReview (1 : n) =====
        modelBuilder.Entity<ProductReview>()
            .HasOne(p=>p.Product)
            .WithMany(pr=>pr.Reviews)
            .HasForeignKey(p=>p.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
