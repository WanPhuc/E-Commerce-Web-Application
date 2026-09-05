using Microsoft.EntityFrameworkCore;
using AuraMart.Shared.Constants;

namespace AuraMart.Ordering.Infrastructure.Persistence.Seeders;

public static class OrderingDemoSeeder
{
    public static async Task SeedAsync(OrderingDbContext context)
    {
        if (await context.Orders.AnyAsync(o => o.Id == DemoIds.PaidOrderId))
            return;

        var now = DateTime.UtcNow;

        context.Orders.AddRange(
            // 1. Order 1 (Delivered & Paid - iPhone 15 Pro Max)
            new Domain.Order
            {
                Id = DemoIds.PaidOrderId,
                UserId = DemoIds.BuyerUserId,
                CustomerName = "Demo Buyer",
                CustomerEmail = DemoIds.BuyerEmail,
                AddressId = DemoIds.BuyerAddressId,
                SellerId = DemoIds.SellerId,
                TotalAmount = 29990000,
                Status = "Delivered",
                PaidAt = now.AddDays(-2),
                CreatedAt = now.AddDays(-3),
                ShippingAddress = new Domain.OrderShippingAddress
                {
                    RecipientName = "Demo Buyer",
                    PhoneNumber = "0900000001",
                    AddressLine = "12 Nguyễn Trãi",
                    Ward = "Phường Bến Thành",
                    District = "Quận 1",
                    City = "TP. Hồ Chí Minh"
                },
                Items =
                [
                    new Domain.OrderItem
                    {
                        Id = DemoIds.PaidOrderItemId,
                        ProductId = DemoIds.IphoneProductId,
                        ProductName = "iPhone 15 Pro Max 256GB Titan Tự Nhiên",
                        Sku = "DEMO-IPHONE-001",
                        Quantity = 1,
                        Price = 29990000,
                        CreatedAt = now.AddDays(-3)
                    }
                ]
            },

            // 2. Order 2 (Processing & Paid - Keyboard + Mouse)
            new Domain.Order
            {
                Id = DemoIds.ProcessingOrderId,
                UserId = DemoIds.BuyerUserId,
                CustomerName = "Demo Buyer",
                CustomerEmail = DemoIds.BuyerEmail,
                AddressId = DemoIds.BuyerAddressId,
                SellerId = DemoIds.Seller2Id,
                TotalAmount = 1589000,
                Status = "Processing",
                PaidAt = now.AddHours(-2),
                CreatedAt = now.AddHours(-3),
                ShippingAddress = new Domain.OrderShippingAddress
                {
                    RecipientName = "Demo Buyer",
                    PhoneNumber = "0900000001",
                    AddressLine = "12 Nguyễn Trãi",
                    Ward = "Phường Bến Thành",
                    District = "Quận 1",
                    City = "TP. Hồ Chí Minh"
                },
                Items =
                [
                    new Domain.OrderItem
                    {
                        Id = DemoIds.ProcessingOrderItemId1,
                        ProductId = DemoIds.KeyboardProductId,
                        ProductName = "Bàn Phím Cơ Không Dây K87 RGB Hot-Swap",
                        Sku = "DEMO-KEYBOARD-001",
                        Quantity = 1,
                        Price = 1190000,
                        CreatedAt = now.AddHours(-3)
                    },
                    new Domain.OrderItem
                    {
                        Id = DemoIds.ProcessingOrderItemId2,
                        ProductId = DemoIds.MouseProductId,
                        ProductName = "Chuột Không Dây Công Thái Học Silent M2",
                        Sku = "DEMO-MOUSE-001",
                        Quantity = 1,
                        Price = 399000,
                        CreatedAt = now.AddHours(-3)
                    }
                ]
            },

            // 3. Order 3 (Pending - T-shirt + Hoodie)
            new Domain.Order
            {
                Id = DemoIds.PendingOrderId,
                UserId = DemoIds.Buyer2UserId,
                CustomerName = "Demo Buyer 2",
                CustomerEmail = DemoIds.Buyer2Email,
                AddressId = DemoIds.Buyer2AddressId,
                SellerId = DemoIds.SellerId,
                TotalAmount = 658000,
                Status = "Pending",
                CreatedAt = now.AddMinutes(-30),
                ShippingAddress = new Domain.OrderShippingAddress
                {
                    RecipientName = "Demo Buyer 2",
                    PhoneNumber = "0900000003",
                    AddressLine = "456 Lê Duẩn",
                    Ward = "Phường Bến Nghé",
                    District = "Quận 1",
                    City = "TP. Hồ Chí Minh"
                },
                Items =
                [
                    new Domain.OrderItem
                    {
                        Id = DemoIds.PendingOrderItemId,
                        ProductId = DemoIds.TshirtProductId,
                        ProductName = "Áo Thun Cotton Nam Classic Form Rộng",
                        Sku = "DEMO-TSHIRT-001",
                        Quantity = 1,
                        Price = 199000,
                        CreatedAt = now.AddMinutes(-30)
                    },
                    new Domain.OrderItem
                    {
                        Id = DemoIds.PendingOrderItemId2,
                        ProductId = DemoIds.HoodieProductId,
                        ProductName = "Áo Khoác Hoodie Unisex Nỉ Bông Streetwear",
                        Sku = "DEMO-HOODIE-001",
                        Quantity = 1,
                        Price = 459000,
                        CreatedAt = now.AddMinutes(-30)
                    }
                ]
            },

            // 4. Order 4 (Cancelled - Desk Lamp)
            new Domain.Order
            {
                Id = DemoIds.CancelledOrderId,
                UserId = DemoIds.BuyerUserId,
                CustomerName = "Demo Buyer",
                CustomerEmail = DemoIds.BuyerEmail,
                AddressId = DemoIds.BuyerAddressId,
                SellerId = DemoIds.SellerId,
                TotalAmount = 329000,
                Status = "Cancelled",
                CreatedAt = now.AddDays(-5),
                ShippingAddress = new Domain.OrderShippingAddress
                {
                    RecipientName = "Demo Buyer",
                    PhoneNumber = "0900000001",
                    AddressLine = "12 Nguyễn Trãi",
                    Ward = "Phường Bến Thành",
                    District = "Quận 1",
                    City = "TP. Hồ Chí Minh"
                },
                Items =
                [
                    new Domain.OrderItem
                    {
                        Id = DemoIds.CancelledOrderItemId,
                        ProductId = DemoIds.DeskLampProductId,
                        ProductName = "Đèn Bàn Học LED Chống Cận Thị 3 Chế Độ Sáng",
                        Sku = "DEMO-LAMP-001",
                        Quantity = 1,
                        Price = 329000,
                        CreatedAt = now.AddDays(-5)
                    }
                ]
            });

        await context.SaveChangesAsync();
    }
}
