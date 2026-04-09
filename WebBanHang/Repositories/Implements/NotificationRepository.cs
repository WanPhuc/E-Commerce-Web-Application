using Microsoft.EntityFrameworkCore;
using WebBanHang.Data;
using WebBanHang.Models.EntityModels;
using WebBanHang.Repositories.Interfaces;

namespace WebBanHang.Repositories.SqlServer;
public class NotificationRepository : SqlServerRepository<Notification>, INotificationRepository
{
    public NotificationRepository(AppDbContext context) : base(context)
    {
    }
    protected override IQueryable<Notification> Query => _dbSet;

    public async Task<List<Notification>> GetNotificationsByReceiverIdAsync(Guid receiverId,int limit = 20)
    {
        return await Query.Where(n => n.ReceiverId == receiverId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }
    public async Task<List<Notification>> GetAllNotificationsByReceiverIdAsync(Guid receiverId)
    {
        return await Query.Where(n=>n.ReceiverId == receiverId)
            .OrderByDescending(n =>n.CreatedAt)
            .ToListAsync();
    }
    public async Task<int> CountUnreadNotificationsAsync(Guid receiverId)
    {
        return await Query.CountAsync(n => n.ReceiverId == receiverId && !n.IsRead);
    }
}