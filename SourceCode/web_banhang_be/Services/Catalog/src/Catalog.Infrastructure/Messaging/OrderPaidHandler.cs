using BuildingBlocks.EventBus;
using Microsoft.EntityFrameworkCore;
using AuraMart.Catalog.Infrastructure.Persistence;

namespace AuraMart.Catalog.Handlers;

public sealed class OrderPaidHandler : IIntegrationEventHandler<OrderPaidIntegrationEvent>
{
    private readonly CatalogDbContext _db;

    public OrderPaidHandler(CatalogDbContext db) => _db = db;

    public async Task HandleAsync(OrderPaidIntegrationEvent evt, CancellationToken ct = default)
    {
        foreach (var item in evt.Items)
        {
            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId, ct);
            if (product == null) continue;

            // Gi?m stock
            product.Stock -= item.Quantity;
            if (product.Stock < 0) product.Stock = 0;

            // Tang soldCount
            product.SoldCount += item.Quantity;
        }
        await _db.SaveChangesAsync(ct);
    }
}
