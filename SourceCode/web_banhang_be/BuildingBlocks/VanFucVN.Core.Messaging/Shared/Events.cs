namespace BuildingBlocks.EventBus;

public record OrderItemEventLine(Guid ProductId, int Quantity);

public sealed class OrderCreatedIntegrationEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }
    public Guid BuyerId { get; init; }
    public Guid SellerId { get; init; }
    public decimal TotalAmount { get; init; }
    public IReadOnlyList<OrderItemEventLine> Items { get; init; } = [];
}

public sealed class OrderPaidIntegrationEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }
    public Guid BuyerId { get; init; }
    public Guid SellerId { get; init; }
    public Guid SellerUserId { get; init; }
    public IReadOnlyList<OrderItemEventLine> Items { get; init; } = [];
}

public sealed class OrderCompletedIntegrationEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }
    public Guid BuyerId { get; init; }
    public Guid SellerId { get; init; }
    public Guid SellerUserId { get; init; }
    public IReadOnlyList<OrderItemEventLine> Items { get; init; } = [];
}

public sealed class OrderCancelledIntegrationEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }
    public Guid BuyerId { get; init; }
    public Guid SellerId { get; init; }
    public Guid SellerUserId { get; init; }
    public string Reason { get; init; } = string.Empty;
    public IReadOnlyList<OrderItemEventLine> Items { get; init; } = [];
}

public sealed class PaymentSucceededIntegrationEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }
    public Guid PaymentId { get; init; }
    public string Method { get; init; } = string.Empty;
    public decimal Amount { get; init; }
}

public sealed class PaymentFailedIntegrationEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }
    public Guid PaymentId { get; init; }
    public string Reason { get; init; } = string.Empty;
}