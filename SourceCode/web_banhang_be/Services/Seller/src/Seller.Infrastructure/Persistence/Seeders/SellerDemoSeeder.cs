using Microsoft.EntityFrameworkCore;
using AuraMart.Seller.Infrastructure.Persistence;
using AuraMart.Shared.Constants;

namespace AuraMart.Seller.Infrastructure.Persistence.Seeders;

public static class SellerDemoSeeder
{
    public static async Task SeedAsync(SellerDbContext context)
    {
        var now = DateTime.UtcNow;

        // Seller 1: AuraMart Official Store
        if (!await context.Sellers.AnyAsync(s => s.Id == DemoIds.SellerId))
        {
            context.Sellers.Add(new Domain.Seller
            {
                Id = DemoIds.SellerId,
                UserId = DemoIds.SellerUserId,
                AddressId = DemoIds.SellerAddressId,
                StoreName = "AuraMart Official Store",
                Description = "Cửa hàng chính hãng phân phối thiết bị công nghệ & thời trang cao cấp",
                Status = "Approved",
                CreatedAt = now
            });
            await context.SaveChangesAsync();
        }

        // Seller 2: TechZone & Fashion Hub
        if (!await context.Sellers.AnyAsync(s => s.Id == DemoIds.Seller2Id))
        {
            context.Sellers.Add(new Domain.Seller
            {
                Id = DemoIds.Seller2Id,
                UserId = DemoIds.Seller2UserId,
                AddressId = DemoIds.Seller2AddressId,
                StoreName = "TechZone & Fashion Hub",
                Description = "Chuyên phụ kiện gaming, thiết bị công nghệ thông minh và thời trang unisex hiện đại",
                Status = "Approved",
                CreatedAt = now
            });
            await context.SaveChangesAsync();
        }

        // Application 1: Buyer 1 applying for new shop (Pending)
        if (!await context.SellerApplications.AnyAsync(a => a.Id == DemoIds.PendingApplicationId))
        {
            context.SellerApplications.Add(new Domain.SellerApplication
            {
                Id = DemoIds.PendingApplicationId,
                UserId = DemoIds.BuyerUserId,
                ShopName = "Buyer Side Shop",
                Description = "Đơn xin mở shop demo đồ thủ công handmade (chờ Admin duyệt)",
                PhoneNumber = "0900000001",
                City = "TP. Hồ Chí Minh",
                District = "Quận 1",
                Ward = "Phường Bến Thành",
                AddressLine = "12 Nguyễn Trãi",
                Status = "Pending",
                CreatedAt = now
            });
            await context.SaveChangesAsync();
        }

        // Application 2: Seller 2 past approved application
        if (!await context.SellerApplications.AnyAsync(a => a.Id == DemoIds.ApprovedApplicationId))
        {
            context.SellerApplications.Add(new Domain.SellerApplication
            {
                Id = DemoIds.ApprovedApplicationId,
                UserId = DemoIds.Seller2UserId,
                ShopName = "TechZone & Fashion Hub",
                Description = "Đơn đăng ký đối tác bán hàng công nghệ & thời trang chính hãng",
                PhoneNumber = "0900000004",
                City = "TP. Hồ Chí Minh",
                District = "Quận 1",
                Ward = "Phường Đa Kao",
                AddressLine = "102 Hai Bà Trưng",
                Status = "Approved",
                CreatedAt = now.AddDays(-30)
            });
            await context.SaveChangesAsync();
        }
    }
}
