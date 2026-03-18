namespace WebBanHang.Services.Global.Hubs;
public interface INotificationService
{
    Task SendNotificationAsync(Guid receiverId, string title, string message, string redirectUrl, NotificationType type);
}