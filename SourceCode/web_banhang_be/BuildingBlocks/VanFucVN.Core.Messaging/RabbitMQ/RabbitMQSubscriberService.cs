using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using BuildingBlocks.EventBus;

namespace BuildingBlocks.RabbitMQ;

public class RabbitMQSubscriberService<TEvent> : BackgroundService where TEvent : IntegrationEvent
{
    private readonly IServiceProvider _sp;
    private readonly ILogger<RabbitMQSubscriberService<TEvent>> _logger;
    private IConnection? _connection;
    private IChannel? _channel;
    private string? _queueName;
    private readonly string _exchange = "auramart_events";
    private readonly string _queueName_const = typeof(TEvent).Name;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public RabbitMQSubscriberService(IServiceProvider sp, ILogger<RabbitMQSubscriberService<TEvent>> logger)
    {
        _sp = sp;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _logger.LogInformation("[RabbitMQ] Starting subscriber for {EventType}", typeof(TEvent).Name);
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await EnsureConnectionAsync(ct);
                await ConsumeAsync(ct);
                await Task.Delay(Timeout.Infinite, ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested) { break; }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[RabbitMQ] Subscriber for {EventType} failed, retrying in 5s", typeof(TEvent).Name);
                try { await Task.Delay(TimeSpan.FromSeconds(5), ct); } catch { break; }
            }
        }
    }

    private async Task EnsureConnectionAsync(CancellationToken ct)
    {
        if (_connection is { IsOpen: true } && _channel is { IsOpen: true })
            return;

        await _lock.WaitAsync(ct);
        try
        {
            if (_connection is { IsOpen: true } && _channel is { IsOpen: true })
                return;

            var factory = _sp.GetRequiredService<IConnectionFactory>();
            _connection = await factory.CreateConnectionAsync(ct);
            _channel = await _connection.CreateChannelAsync(cancellationToken: ct);

            await _channel.ExchangeDeclareAsync(exchange: _exchange, type: ExchangeType.Fanout, durable: true, cancellationToken: ct);
            _queueName = await _channel.QueueDeclareAsync(queue: _queueName_const, durable: true, exclusive: false, autoDelete: false, cancellationToken: ct);
            await _channel.QueueBindAsync(queue: _queueName, exchange: _exchange, routingKey: "", cancellationToken: ct);

            _logger.LogInformation("[RabbitMQ] Connected and queue {Queue} bound to {Exchange}", _queueName, _exchange);
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task ConsumeAsync(CancellationToken ct)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel!);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var evt = JsonSerializer.Deserialize<TEvent>(json);
                if (evt != null)
                {
                    using var scope = _sp.CreateScope();
                    var handler = scope.ServiceProvider.GetRequiredService<IIntegrationEventHandler<TEvent>>();
                    await handler.HandleAsync(evt, ct);
                }
                await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[RabbitMQ] Error handling {EventType}", typeof(TEvent).Name);
                await _channel!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        await _channel!.BasicConsumeAsync(queue: _queueName!, autoAck: false, consumer: consumer);
        _logger.LogInformation("[RabbitMQ] Consuming from queue {Queue}", _queueName);
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        _lock.Dispose();
        base.Dispose();
    }
}
