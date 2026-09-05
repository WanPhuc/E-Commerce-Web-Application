using BuildingBlocks.EventBus;
using Microsoft.EntityFrameworkCore;
using Serilog;
using AuraMart.Ordering.Infrastructure.Persistence;

namespace AuraMart.Ordering.Api.Handlers;

public class PaymentFailedHandler : IIntegrationEventHandler<PaymentFailedIntegrationEvent>
{
    private readonly OrderingDbContext _db;

    public PaymentFailedHandler(OrderingDbContext db) { _db = db; }

    public async Task HandleAsync(PaymentFailedIntegrationEvent evt, CancellationToken ct = default)
    {
        var order = await _db.Orders.FindAsync(evt.OrderId);
        if (order != null && order.Status != "Cancelled")
        {
            order.Status = "Cancelled";
            order.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            Log.Information("[Ordering] Order {OrderId} cancelled via RabbitMQ (payment failed)", evt.OrderId);
        }
    }
}
