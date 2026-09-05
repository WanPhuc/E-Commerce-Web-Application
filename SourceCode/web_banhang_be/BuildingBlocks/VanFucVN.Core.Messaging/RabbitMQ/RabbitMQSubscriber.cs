using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using BuildingBlocks.EventBus;

namespace BuildingBlocks.RabbitMQ;

public class RabbitMQSubscriber<TEvent> : IDisposable where TEvent : IntegrationEvent
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly IIntegrationEventHandler<TEvent> _handler;
    private IConnection? _connection;
    private IChannel? _channel;
    private string? _queueName;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public RabbitMQSubscriber(IConnectionFactory connectionFactory, IIntegrationEventHandler<TEvent> handler)
    {
        _connectionFactory = connectionFactory;
        _handler = handler;
    }

    public async Task StartAsync(CancellationToken ct = default)
    {
        await EnsureConnectionAsync();

        // Declare queue (durable, not auto-delete)
        _queueName = await _channel!.QueueDeclareAsync(
            queue: typeof(TEvent).Name,
            durable: true,
            exclusive: false,
            autoDelete: false);

        // Bind queue to exchange
        await _channel.QueueBindAsync(
            queue: _queueName,
            exchange: "auramart_events",
            routingKey: "");

        // Start consuming
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var evt = JsonSerializer.Deserialize<TEvent>(json);
                if (evt != null)
                {
                    await _handler.HandleAsync(evt, ct);
                    await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RabbitMQ] Error handling event {typeof(TEvent).Name}: {ex.Message}");
                await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: _queueName,
            autoAck: false,
            consumer: consumer);
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

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        _lock.Dispose();
    }
}
