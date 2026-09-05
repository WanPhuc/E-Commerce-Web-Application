namespace AuraMart.Notification.Domain.Repositories;

public interface INotificationRepository : IBaseRepository<Domain.Notification>
{
    Task<List<Domain.Notification>> GetByReceiverIdAsync(Guid receiverId);
}
