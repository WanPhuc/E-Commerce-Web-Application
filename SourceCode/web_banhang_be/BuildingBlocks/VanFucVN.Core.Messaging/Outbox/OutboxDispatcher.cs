using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace BuildingBlocks.Outbox;

/// <summary>
/// Generic OutboxDispatcher — dùng được với bất kỳ DbContext nào có DbSet&lt;OutboxMessage&gt;.
/// Đăng ký trong Program.cs: services.AddOutboxDispatcher&lt;TDbContext&gt;("ServiceName");
/// </summary>
public sealed class OutboxDispatcher<TDbContext> : BackgroundService
    where TDbContext : DbContext
{
    private readonly IServiceProvider _sp;
    private readonly IConnectionFactory _rabbitFactory;
    private readonly string _serviceName;
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(2);
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private IConnection? _connection;
    private IChannel? _channel;

    public OutboxDispatcher(IServiceProvider sp, IConnectionFactory rabbitFactory, string serviceName)
    {
        _sp = sp;
        _rabbitFactory = rabbitFactory;
        _serviceName = serviceName;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        Console.WriteLine($"[{_serviceName}] OutboxDispatcher started (RabbitMQ)");
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await EnsureRabbitConnectionAsync(ct);
                await DispatchPendingAsync(ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested) { break; }
            catch (Exception ex) { Console.WriteLine($"[{_serviceName}] OutboxDispatcher error: {ex.Message}"); }
            try { await Task.Delay(PollInterval, ct); }
            catch (OperationCanceledException) { break; }
        }
    }

    private async Task EnsureRabbitConnectionAsync(CancellationToken ct)
    {
        if (_connection is { IsOpen: true } && _channel is { IsOpen: true }) return;

        _connection = await _rabbitFactory.CreateConnectionAsync(ct);
        _channel = await _connection.CreateChannelAsync(cancellationToken: ct);
        await _channel.ExchangeDeclareAsync(
            exchange: "auramart_events",
            type: ExchangeType.Fanout,
            durable: true,
            cancellationToken: ct);
        Console.WriteLine($"[{_serviceName}] RabbitMQ connected");
    }

    private async Task DispatchPendingAsync(CancellationToken ct)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TDbContext>();

        var pending = await db.Set<OutboxMessage>()
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(20)
            .ToListAsync(ct);

        foreach (var msg in pending)
        {
            try
            {
                var eventType = Type.GetType(msg.Type);
                if (eventType == null) continue;

                var body = System.Text.Encoding.UTF8.GetBytes(msg.Payload);
                var properties = new BasicProperties
                {
                    Headers = new Dictionary<string, object?> { ["event-type"] = eventType.Name },
                    ContentType = "application/json",
                    DeliveryMode = DeliveryModes.Persistent
                };

                await _channel!.BasicPublishAsync(
                    exchange: "auramart_events",
                    routingKey: "",
                    mandatory: false,
                    basicProperties: properties,
                    body: body,
                    cancellationToken: ct);

                msg.ProcessedOnUtc = DateTime.UtcNow;
                await db.SaveChangesAsync(ct);
                Console.WriteLine($"[{_serviceName}] Published {eventType.Name} to RabbitMQ");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{_serviceName}] Failed dispatching {msg.Id}: {ex.Message}");
            }
        }
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}

/// <summary>
/// Extension method để đăng ký OutboxDispatcher generic dễ dàng.
/// </summary>
public static class OutboxServiceExtensions
{
    public static IServiceCollection AddOutboxDispatcher<TDbContext>(
        this IServiceCollection services,
        string serviceName)
        where TDbContext : DbContext
    {
        services.AddSingleton(sp => new OutboxDispatcher<TDbContext>(
            sp,
            sp.GetRequiredService<IConnectionFactory>(),
            serviceName));
        services.AddHostedService(sp => sp.GetRequiredService<OutboxDispatcher<TDbContext>>());
        return services;
    }
}
