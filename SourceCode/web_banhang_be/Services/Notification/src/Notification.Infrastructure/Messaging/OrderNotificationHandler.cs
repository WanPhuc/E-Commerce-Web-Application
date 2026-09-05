using BuildingBlocks.EventBus;

namespace AuraMart.Notification.Infrastructure.Messaging;

public class OrderNotificationHandler :
    IIntegrationEventHandler<OrderPaidIntegrationEvent>,
    IIntegrationEventHandler<OrderCompletedIntegrationEvent>,
    IIntegrationEventHandler<OrderCancelledIntegrationEvent>
{
    private readonly INotificationService _notificationService;

    public OrderNotificationHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task HandleAsync(OrderPaidIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        await _notificationService.CreateNotificationAsync(new CreateNotificationRequest
        {
            ReceiverId = @event.BuyerId,
            Title = "Đơn hàng đã được thanh toán",
            Message = $"Đơn hàng {@event.OrderId} đã thanh toán thành công.",
            Type = NotificationType.Order
        });

        await _notificationService.CreateNotificationAsync(new CreateNotificationRequest
        {
            ReceiverId = @event.SellerUserId,
            Title = "Đơn hàng mới",
            Message = $"Bạn có đơn hàng mới {@event.OrderId} đã được thanh toán.",
            Type = NotificationType.Order
        });
    }

    public async Task HandleAsync(OrderCompletedIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        await _notificationService.CreateNotificationAsync(new CreateNotificationRequest
        {
            ReceiverId = @event.BuyerId,
            Title = "Đơn hàng hoàn tất",
            Message = $"Đơn hàng {@event.OrderId} đã giao thành công.",
            Type = NotificationType.Order
        });
    }

    public async Task HandleAsync(OrderCancelledIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        await _notificationService.CreateNotificationAsync(new CreateNotificationRequest
        {
            ReceiverId = @event.BuyerId,
            Title = "Đơn hàng đã hủy",
            Message = $"Đơn hàng {@event.OrderId} đã bị hủy. Lý do: {@event.Reason}",
            Type = NotificationType.Order
        });
    }
}
