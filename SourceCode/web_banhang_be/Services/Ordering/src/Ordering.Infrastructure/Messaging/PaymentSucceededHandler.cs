using BuildingBlocks.EventBus;
using Microsoft.EntityFrameworkCore;
using Serilog;
using AuraMart.Ordering.Infrastructure.Persistence;

namespace AuraMart.Ordering.Api.Handlers;

public class PaymentSucceededHandler : IIntegrationEventHandler<PaymentSucceededIntegrationEvent>
{
    private readonly OrderingDbContext _db;

    public PaymentSucceededHandler(OrderingDbContext db) { _db = db; }

    public async Task HandleAsync(PaymentSucceededIntegrationEvent evt, CancellationToken ct = default)
    {
        var order = await _db.Orders.FindAsync(evt.OrderId);
        if (order != null && order.Status != "Paid")
        {
            order.Status = "Paid";
            order.PaidAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            Log.Information("[Ordering] Order {OrderId} marked Paid via RabbitMQ", evt.OrderId);
        }
    }
}
