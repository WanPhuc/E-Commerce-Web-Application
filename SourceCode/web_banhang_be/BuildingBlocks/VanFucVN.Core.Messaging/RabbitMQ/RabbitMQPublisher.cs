using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using BuildingBlocks.EventBus;

namespace BuildingBlocks.RabbitMQ;

public class RabbitMQPublisher : IDisposable
{
    private readonly IConnectionFactory _connectionFactory;
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public RabbitMQPublisher(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    private async Task EnsureConnectionAsync()
    {
        if (_connection is { IsOpen: true } && _channel is { IsOpen: true })
            return;

        await _lock.WaitAsync();
        try
        {
            if (_connection is { IsOpen: true } && _channel is { IsOpen: true })
                return;

            _connection = await _connectionFactory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            // Declare exchange for integration events
            await _channel.ExchangeDeclareAsync(
                exchange: "auramart_events",
                type: ExchangeType.Fanout,
                durable: true);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task PublishAsync<T>(T integrationEvent, CancellationToken ct = default) where T : IntegrationEvent
    {
        await EnsureConnectionAsync();

        var eventName = typeof(T).Name;
        var json = JsonSerializer.Serialize(integrationEvent);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = new BasicProperties
        {
            Headers = new Dictionary<string, object?>
            {
                ["event-type"] = eventName
            },
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent
        };

        await _channel!.BasicPublishAsync(
            exchange: "auramart_events",
            routingKey: "",
            mandatory: false,
            basicProperties: properties,
            body: body);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        _lock.Dispose();
    }
}
