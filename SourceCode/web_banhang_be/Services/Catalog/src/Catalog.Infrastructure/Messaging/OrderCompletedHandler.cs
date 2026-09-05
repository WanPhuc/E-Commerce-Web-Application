using BuildingBlocks.EventBus;
using AuraMart.Catalog.Infrastructure.Persistence;

namespace AuraMart.Catalog.Handlers;

public sealed class OrderCompletedHandler : IIntegrationEventHandler<OrderCompletedIntegrationEvent>
{
    public Task HandleAsync(OrderCompletedIntegrationEvent evt, CancellationToken ct = default)
        => Task.CompletedTask;
}
