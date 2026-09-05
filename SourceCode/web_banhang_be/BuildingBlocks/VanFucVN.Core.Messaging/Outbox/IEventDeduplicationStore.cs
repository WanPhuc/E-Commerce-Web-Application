namespace BuildingBlocks.EventBus;

public interface IEventDeduplicationStore
{
    Task<bool> TryMarkProcessedAsync(Guid eventId, CancellationToken ct = default);
}