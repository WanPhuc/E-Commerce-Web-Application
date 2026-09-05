using Microsoft.EntityFrameworkCore;
using AuraMart.Shared.Constants;

namespace AuraMart.Payment.Infrastructure.Persistence.Seeders;

public static class PaymentDemoSeeder
{
    public static async Task SeedAsync(PaymentDbContext context)
    {
        if (await context.Payments.AnyAsync(p => p.Id == DemoIds.PaidPaymentId))
            return;

        var now = DateTime.UtcNow;

        context.Payments.AddRange(
            // 1. Payment for Order 1 (COD - Succeeded)
            new Domain.Payment
            {
                Id = DemoIds.PaidPaymentId,
                OrderId = DemoIds.PaidOrderId,
                Method = "COD",
                Amount = 29990000,
                Status = "Succeeded",
                CreatedAt = now.AddDays(-2)
            },

            // 2. Payment for Order 2 (VNPay - Succeeded)
            new Domain.Payment
            {
                Id = DemoIds.ProcessingPaymentId,
                OrderId = DemoIds.ProcessingOrderId,
                Method = "VNPay",
                Amount = 1589000,
                Status = "Succeeded",
                CreatedAt = now.AddHours(-2)
            },

            // 3. Payment for Order 3 (COD - Pending)
            new Domain.Payment
            {
                Id = DemoIds.PendingPaymentId,
                OrderId = DemoIds.PendingOrderId,
                Method = "COD",
                Amount = 658000,
                Status = "Pending",
                CreatedAt = now.AddMinutes(-30)
            });

        await context.SaveChangesAsync();
    }
}
