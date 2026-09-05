using Microsoft.EntityFrameworkCore;
using AuraMart.Identity.Infrastructure.Persistence;
using AuraMart.Shared.Constants;
using AuraMart.Identity.Application;
using AuraMart.Identity.Domain;

namespace WebBanHang.Data.Seeders;

public static class DemoIdentitySeeder
{
    public static async Task SeedAsync(IdentityDbContext db)
    {
        // Seller 1
        await EnsureUserAsync(
            db,
            DemoIds.SellerUserId,
            DemoIds.SellerEmail,
            "AuraMart Official Store",
            "Seller",
            DemoIds.SellerAddressId,
            "0900000002",
            "88 Lê Lợi",
            "Bến Nghé",
            "Quận 1",
            "TP. Hồ Chí Minh");

        // Seller 2
        await EnsureUserAsync(
            db,
            DemoIds.Seller2UserId,
            DemoIds.Seller2Email,
            "TechZone & Fashion Hub",
            "Seller",
            DemoIds.Seller2AddressId,
            "0900000004",
            "102 Hai Bà Trưng",
            "Đa Kao",
            "Quận 1",
            "TP. Hồ Chí Minh");

        // Buyer 1
        await EnsureUserAsync(
            db,
            DemoIds.BuyerUserId,
            DemoIds.BuyerEmail,
            "Demo Buyer",
            "Buyer",
            DemoIds.BuyerAddressId,
            "0900000001",
            "12 Nguyễn Trãi",
            "Bến Thành",
            "Quận 1",
            "TP. Hồ Chí Minh");

        // Buyer 2
        await EnsureUserAsync(
            db,
            DemoIds.Buyer2UserId,
            DemoIds.Buyer2Email,
            "Demo Buyer 2",
            "Buyer",
            DemoIds.Buyer2AddressId,
            "0900000003",
            "456 Lê Duẩn",
            "Bến Nghé",
            "Quận 1",
            "TP. Hồ Chí Minh");
    }

    private static async Task EnsureUserAsync(
        IdentityDbContext db,
        Guid userId,
        string email,
        string fullName,
        string roleName,
        Guid addressId,
        string phone,
        string addressLine,
        string ward,
        string district,
        string city)
    {
        if (await db.Users.AnyAsync(u => u.Id == userId || u.Email == email))
            return;

        var role = await db.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        if (role == null) return;

        var now = DateTime.UtcNow;
        db.Users.Add(new User
        {
            Id = userId,
            ProviderType = 0,
            FullName = fullName,
            Email = email,
            PasswordHash = PasswordHelper.HashPassword(DemoIds.DemoPassword),
            IsActive = true,
            RoleId = role.Id,
            CreatedAt = now,
            UpdatedAt = now
        });
        await db.SaveChangesAsync();

        if (!await db.Addresses.AnyAsync(a => a.Id == addressId))
        {
            db.Addresses.Add(new Address
            {
                Id = addressId,
                UserId = userId,
                RecipientName = fullName,
                PhoneNumber = phone,
                AddressLine = addressLine,
                Ward = ward,
                District = district,
                City = city,
                IsDefault = true,
                CreatedAt = now,
                UpdatedAt = now
            });
            await db.SaveChangesAsync();
        }
    }
}
