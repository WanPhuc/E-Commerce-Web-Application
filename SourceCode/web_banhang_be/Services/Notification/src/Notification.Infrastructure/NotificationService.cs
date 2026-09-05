using Microsoft.AspNetCore.SignalR;
using AuraMart.Notification.Infrastructure.Realtime;

namespace AuraMart.Notification.Infrastructure;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notifRepo;
    private readonly NotificationDbContext _db;
    private readonly IHubContext<GlobalHub> _hub;

    public NotificationService(
        INotificationRepository notifRepo,
        NotificationDbContext db,
        IHubContext<GlobalHub> hub)
    {
        _notifRepo = notifRepo;
        _db = db;
        _hub = hub;
    }

    public async Task<ApiResponse<List<NotificationDto>>> GetUserNotificationsAsync(Guid userId)
    {
        var list = await _notifRepo.GetByReceiverIdAsync(userId);
        var dtos = list.Select(MapToDto).ToList();
        return ApiResponse<List<NotificationDto>>.Success(dtos);
    }

    public async Task<ApiResponse<NotificationDto>> CreateNotificationAsync(CreateNotificationRequest request)
    {
        var notif = new Domain.Notification
        {
            Id = Guid.NewGuid(),
            ReceiverId = request.ReceiverId,
            Title = request.Title,
            Message = request.Message,
            Type = request.Type,
            RedirectUrl = request.RedirectUrl,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _db.Notifications.Add(notif);
        await _db.SaveChangesAsync();

        var dto = MapToDto(notif);

        // Realtime SignalR notification
        await _hub.Clients.User(request.ReceiverId.ToString()).SendAsync("ReceiveNotification", dto);

        return ApiResponse<NotificationDto>.Success(dto, "Tạo thông báo thành công", 201);
    }

    public async Task<ApiResponse<bool>> MarkAsReadAsync(Guid notificationId, Guid userId)
    {
        var notif = await _notifRepo.GetByIdAsync(notificationId);
        if (notif == null || notif.ReceiverId != userId)
        {
            return ApiResponse<bool>.Fail("Không tìm thấy thông báo", 404);
        }

        notif.IsRead = true;
        notif.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return ApiResponse<bool>.Success(true, "Đã đánh dấu đã đọc");
    }

    private static NotificationDto MapToDto(Domain.Notification n) => new()
    {
        Id = n.Id,
        ReceiverId = n.ReceiverId,
        Title = n.Title,
        Message = n.Message,
        Type = n.Type.ToString(),
        RedirectUrl = n.RedirectUrl,
        IsRead = n.IsRead,
        CreatedAt = n.CreatedAt
    };
}
