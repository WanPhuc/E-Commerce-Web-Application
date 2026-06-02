using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Global;

namespace WebBanHang.Services.Global.Interfaces;
public interface INotificationService
{
    Task SendNotificationAsync(Guid receiverId, string title, string message, string redirectUrl, NotificationType type);
    Task<ApiResponse<PagedResult<NotificationDto>>> GetMyNotifications(PagedRequest request);
    Task<ApiResponse<int>> GetUnreadNotificationCount();
    Task<ApiResponse> MarkAsRead(Guid notiId);
}   
