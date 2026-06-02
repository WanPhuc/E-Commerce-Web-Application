using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebBanHang.Extensions;
using WebBanHang.Helpers.Hubs;
using WebBanHang.Models;
using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Global;
using WebBanHang.Models.EntityModels;
using WebBanHang.Repositories.Interfaces;
using WebBanHang.Services.Global.Interfaces;

namespace WebBanHang.Services.Global.Implements;

public class NotificationService : INotificationService
{
    private readonly IHubContext<GlobalHub> _hubContext;
    private readonly INotificationRepository _notificationRepository;
    private readonly ICommonService _commonService;
    public NotificationService(IHubContext<GlobalHub> hubContext, INotificationRepository notificationRepository, ICommonService commonService)
    {
        _hubContext = hubContext;
        _notificationRepository = notificationRepository;
        _commonService = commonService;
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
        await _notificationRepository.CreateAsync(notification);
        await _notificationRepository.SaveChangesAsync();

        await _hubContext.Clients.User(receiverId.ToString()).SendAsync("ReceiveNotification", new
        {
            id = notification.Id,
            title,
            message,
            redirectUrl,
            type = type.ToString(),
            createdAt = notification.CreatedAt,
        });
    }
    public async Task<ApiResponse<PagedResult<NotificationDto>>> GetMyNotifications(PagedRequest request)
    {
        var userId = _commonService.GetUserId();
        var noti = await _notificationRepository
            .FindByCondition(n => n.ReceiverId == userId && !n.DeleteFlg)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                RedirectUrl = n.RedirectUrl,
                Type = n.Type,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            }).ToPagedAsync(request);
        return ApiResponse<PagedResult<NotificationDto>>.Success(noti, "Success", 200, SuccessCodes.Notification.ListRetrieved);
    }
    public async Task<ApiResponse<int>> GetUnreadNotificationCount()
    {
        var userId = _commonService.GetUserId();
        var count = await _notificationRepository.CountByConditionAsync(n => n.ReceiverId == userId && !n.IsRead && !n.DeleteFlg);
        return ApiResponse<int>.Success(count, "Success", 200, SuccessCodes.Notification.CountRetrieved);
    }
    public async Task<ApiResponse> MarkAsRead(Guid notiId)
    {
        var notification = await _notificationRepository.GetByIdAsync(notiId);
        if (notification == null) return ApiResponse.Fail("Notification not found.", 404, ErrorCodes.Notification.NotFound);
        var userId = _commonService.GetUserId();
        if (notification.ReceiverId != userId) return ApiResponse.Fail("Unauthorized.", 401, ErrorCodes.Notification.Unauthorized);
        notification.IsRead = true;
        await _notificationRepository.UpdateAsync(notification);
        return ApiResponse.Success("Success", 200, SuccessCodes.Notification.MarkedAsRead);
    }

}
