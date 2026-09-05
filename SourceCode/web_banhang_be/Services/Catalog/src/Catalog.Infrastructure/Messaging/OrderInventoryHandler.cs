using BuildingBlocks.EventBus;
using Microsoft.EntityFrameworkCore;
using Serilog;


namespace AuraMart.Catalog.Services;

// Consumer cua Ordering events - cap nhat ton kho & sold count.
// Idempotent nho dedup store o dispatcher (1 EventId chay 1 lan).
public class OrderInventoryHandler(CatalogDbContext db) :
    IIntegrationEventHandler<OrderPaidIntegrationEvent>,
    IIntegrationEventHandler<OrderCompletedIntegrationEvent>,
    IIntegrationEventHandler<OrderCancelledIntegrationEvent>
{
    public async Task HandleAsync(OrderPaidIntegrationEvent evt, CancellationToken ct = default)
    {
        var ids = evt.Items.Select(i => i.ProductId).ToList();
        var products = await db.Products.Where(p => ids.Contains(p.Id)).ToListAsync(ct);
        foreach (var line in evt.Items)
        {
            var p = products.FirstOrDefault(x => x.Id == line.ProductId);
            if (p == null) continue;
            p.Stock = Math.Max(0, p.Stock - line.Quantity);
        }
        await db.SaveChangesAsync(ct);
        Log.Information("[Catalog] Stock reserved for order {OrderId} ({Count} lines)", evt.OrderId, evt.Items.Count);
    }

    public async Task HandleAsync(OrderCompletedIntegrationEvent evt, CancellationToken ct = default)
    {
        var ids = evt.Items.Select(i => i.ProductId).ToList();
        var products = await db.Products.Where(p => ids.Contains(p.Id)).ToListAsync(ct);
        foreach (var line in evt.Items)
        {
            var p = products.FirstOrDefault(x => x.Id == line.ProductId);
            if (p == null) continue;
            p.SoldCount += line.Quantity;
        }
        await db.SaveChangesAsync(ct);
        Log.Information("[Catalog] SoldCount incremented for order {OrderId} ({Count} lines)", evt.OrderId, evt.Items.Count);
    }

    public async Task HandleAsync(OrderCancelledIntegrationEvent evt, CancellationToken ct = default)
    {
        // Hoan stock chi khi don tung duoc thanh toan (da tru stock) -
        // event Items rong neu chua Paid nen loop khong chay
        if (evt.Items.Count == 0) return;
        var ids = evt.Items.Select(i => i.ProductId).ToList();
        var products = await db.Products.Where(p => ids.Contains(p.Id)).ToListAsync(ct);
        foreach (var line in evt.Items)
        {
            var p = products.FirstOrDefault(x => x.Id == line.ProductId);
            if (p == null) continue;
            p.Stock += line.Quantity;
        }
        await db.SaveChangesAsync(ct);
        Log.Information("[Catalog] Stock restored for cancelled order {OrderId}", evt.OrderId);
    }
}
