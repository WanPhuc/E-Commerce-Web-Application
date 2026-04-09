using WebBanHang.Models.EntityModels;

namespace WebBanHang.Repositories.Interfaces;
public interface INotificationRepository : IRepository<Notification>
{
    Task<List<Notification>> GetNotificationsByReceiverIdAsync(Guid receiverId,int limit = 20);
    Task<List<Notification>> GetAllNotificationsByReceiverIdAsync(Guid receiverId);
    Task<int> CountUnreadNotificationsAsync(Guid receiverId);
}