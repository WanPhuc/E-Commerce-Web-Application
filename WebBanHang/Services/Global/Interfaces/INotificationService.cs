namespace WebBanHang.Services.Global.Interfaces;
public interface INotificationService
{
    Task SendNotificationAsync(Guid receiverId, string title, string message, string redirectUrl, NotificationType type);
}