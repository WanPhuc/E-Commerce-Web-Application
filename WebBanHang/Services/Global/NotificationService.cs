using Microsoft.AspNetCore.SignalR;
using WebBanHang.Helpers.Hubs;
using WebBanHang.Models;
using WebBanHang.Repositories;

namespace WebBanHang.Services.Global.Hubs;
public class NotificationService : INotificationService
{
    private readonly IHubContext<GlobalHub> _hubContext;
    private readonly INotificationRepository _notificationRepository;
    public NotificationService(IHubContext<GlobalHub> hubContext, INotificationRepository notificationRepository)
    {
        _hubContext = hubContext;
        _notificationRepository = notificationRepository;
    }
    public async Task SendNotificationAsync(Guid receiverId, string title, string message, string redirectUrl, NotificationType type)
    {
        var notification = new Notification
        {
            ReceiverId = receiverId,
            Title = title,
            Message = message,
            RedirectUrl = redirectUrl,
            Type = type,
        };
        await _notificationRepository.AddAsync(notification);
        await _notificationRepository.SaveChangesAsync();

        await _hubContext.Clients.User(receiverId.ToString()).SendAsync("ReceiveNotification", new
        {
            id = notification.Id,
            title,
            message,
            redirectUrl,
            type=type.ToString(),
            createdAt = notification.CreatedAt,
        });
        
    }

}