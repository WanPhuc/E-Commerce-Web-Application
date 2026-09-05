using System.Text.Json;
using BuildingBlocks.EventBus;

namespace BuildingBlocks.Outbox;

public interface IOutboxWriter
{
    void Add(IntegrationEvent integrationEvent);
}

/// <summary>
/// Generic OutboxWriter hoạt động với bất kỳ DbContext nào có DbSet&lt;OutboxMessage&gt;.
/// Inject IOutboxWriterFor&lt;TDbContext&gt; vào Service Layer để dùng.
/// </summary>
public interface IOutboxWriterFor<TDbContext> : IOutboxWriter
{
    Task SaveAsync(TDbContext db, CancellationToken ct = default);
}

public class OutboxWriter<TDbContext> : IOutboxWriterFor<TDbContext>
    where TDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    private readonly List<OutboxMessage> _pending = [];
    private readonly string _serviceName;

    public OutboxWriter(string serviceName = "Service")
    {
        _serviceName = serviceName;
    }

    public void Add(IntegrationEvent evt)
    {
        _pending.Add(new OutboxMessage
        {
            Id = evt.EventId,
            Type = evt.GetType().AssemblyQualifiedName ?? evt.GetType().Name,
            Payload = JsonSerializer.Serialize(evt, evt.GetType()),
            OccurredOnUtc = evt.OccurredOnUtc
        });
    }

    public async Task SaveAsync(TDbContext db, CancellationToken ct = default)
    {
        var set = db.Set<OutboxMessage>();
        set.AddRange(_pending);
        await db.SaveChangesAsync(ct);
        _pending.Clear();
    }
}