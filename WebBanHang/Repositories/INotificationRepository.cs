using WebBanHang.Models;

namespace WebBanHang.Repositories;
public interface INotificationRepository : IRepository<Notification>
{
    Task<List<Notification>> GetNotificationsByReceiverIdAsync(Guid receiverId,int limit = 20);
    Task<int> CountUnreadNotificationsAsync(Guid receiverId);
}