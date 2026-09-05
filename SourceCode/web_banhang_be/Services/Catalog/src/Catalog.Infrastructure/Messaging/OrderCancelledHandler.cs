using BuildingBlocks.EventBus;
using Microsoft.EntityFrameworkCore;
using AuraMart.Catalog.Infrastructure.Persistence;

namespace AuraMart.Catalog.Handlers;

public sealed class OrderCancelledHandler : IIntegrationEventHandler<OrderCancelledIntegrationEvent>
{
    private readonly CatalogDbContext _db;

    public OrderCancelledHandler(CatalogDbContext db) => _db = db;

    public async Task HandleAsync(OrderCancelledIntegrationEvent evt, CancellationToken ct = default)
    {
        foreach (var item in evt.Items)
        {
            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId, ct);
            if (product == null) continue;
            product.Stock += item.Quantity;
        }
        await _db.SaveChangesAsync(ct);
    }
}
