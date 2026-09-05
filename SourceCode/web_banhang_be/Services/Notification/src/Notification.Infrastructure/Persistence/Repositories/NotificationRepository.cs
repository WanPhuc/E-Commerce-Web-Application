using Microsoft.EntityFrameworkCore;

namespace AuraMart.Notification.Infrastructure.Persistence.Repositories;

public class NotificationRepository : BaseRepository<Domain.Notification, NotificationDbContext>, INotificationRepository
{
    public NotificationRepository(NotificationDbContext context) : base(context) { }

    public async Task<List<Domain.Notification>> GetByReceiverIdAsync(Guid receiverId)
    {
        return await _db.Notifications
            .Where(n => n.ReceiverId == receiverId && !n.DeleteFlg)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }
}
