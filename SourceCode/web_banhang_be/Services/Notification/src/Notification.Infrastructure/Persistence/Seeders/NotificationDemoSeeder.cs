using Microsoft.EntityFrameworkCore;
using AuraMart.Shared.Constants;

namespace AuraMart.Notification.Infrastructure.Persistence.Seeders;

public static class NotificationDemoSeeder
{
    public static async Task SeedAsync(NotificationDbContext context)
    {
        if (await context.Notifications.AnyAsync(n => n.Id == DemoIds.BuyerNotificationId))
            return;

        var now = DateTime.UtcNow;
        context.Notifications.AddRange(
            // 1. Buyer 1 - Order 1 Delivered
            new Domain.Notification
            {
                Id = DemoIds.BuyerNotificationId,
                ReceiverId = DemoIds.BuyerUserId,
                Title = "Giao hàng thành công",
                Message = "Đơn hàng iPhone 15 Pro Max 256GB đã được giao thành công đến bạn.",
                RedirectUrl = $"/orders/{DemoIds.PaidOrderId}",
                Type = Domain.NotificationType.Order,
                IsRead = true,
                CreatedAt = now.AddDays(-2)
            },

            // 2. Buyer 1 - Order 2 Paid
            new Domain.Notification
            {
                Id = DemoIds.BuyerNotification2Id,
                ReceiverId = DemoIds.BuyerUserId,
                Title = "Thanh toán VNPay thành công",
                Message = "Đơn hàng Bàn phím cơ K87 & Chuột M2 (1.589.000đ) đã thanh toán thành công và đang được chuẩn bị.",
                RedirectUrl = $"/orders/{DemoIds.ProcessingOrderId}",
                Type = Domain.NotificationType.Order,
                IsRead = false,
                CreatedAt = now.AddHours(-2)
            },

            // 3. Buyer 2 - Order 3 Placed
            new Domain.Notification
            {
                Id = DemoIds.Buyer2NotificationId,
                ReceiverId = DemoIds.Buyer2UserId,
                Title = "Đặt hàng thành công",
                Message = "Đơn hàng Áo thun & Hoodie của bạn đã được ghi nhận. Shop đang xử lý đơn.",
                RedirectUrl = $"/orders/{DemoIds.PendingOrderId}",
                Type = Domain.NotificationType.Order,
                IsRead = false,
                CreatedAt = now.AddMinutes(-30)
            },

            // 4. Seller 1 - Order 3 New Order
            new Domain.Notification
            {
                Id = DemoIds.SellerNotificationId,
                ReceiverId = DemoIds.SellerUserId,
                Title = "Đơn hàng mới chờ xác nhận",
                Message = "Khách hàng Demo Buyer 2 vừa đặt đơn hàng mới gồm Áo thun nam Cotton.",
                RedirectUrl = $"/seller/orders/{DemoIds.PendingOrderId}",
                Type = Domain.NotificationType.Order,
                IsRead = false,
                CreatedAt = now.AddMinutes(-30)
            },

            // 5. Seller 1 - Low Stock Warning
            new Domain.Notification
            {
                Id = DemoIds.SellerNotification2Id,
                ReceiverId = DemoIds.SellerUserId,
                Title = "Cảnh báo tồn kho",
                Message = "Sản phẩm 'Nồi Chiên Không Dầu Điện Tử 6.5L' sắp chạm ngưỡng tồn kho tối thiểu.",
                RedirectUrl = $"/seller/products/{DemoIds.AirFryerProductId}",
                Type = Domain.NotificationType.System,
                IsRead = false,
                CreatedAt = now.AddHours(-5)
            },

            // 6. Seller 2 - Order 2 New Order (Paid)
            new Domain.Notification
            {
                Id = DemoIds.Seller2NotificationId,
                ReceiverId = DemoIds.Seller2UserId,
                Title = "Đơn hàng mới đã thanh toán",
                Message = "Khách hàng Demo Buyer đã thanh toán đơn Bàn phím cơ K87 & Chuột M2.",
                RedirectUrl = $"/seller/orders/{DemoIds.ProcessingOrderId}",
                Type = Domain.NotificationType.Order,
                IsRead = false,
                CreatedAt = now.AddHours(-2)
            });

        await context.SaveChangesAsync();
    }
}
