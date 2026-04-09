using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Global;
using WebBanHang.Repositories.Interfaces;

namespace WebBanHang.Controllers.Api.Global;

[ApiController]
[Route("api/v1/global/notifications")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationRepository _notificationRepository;
    public NotificationController( INotificationRepository notificationRepository)
    {
        
        _notificationRepository = notificationRepository;
    }
    [HttpGet]
    public async Task<ActionResult<ApiResponse<NotificationDto>>> GetMyNotifications()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized("Not found user information.");
        Guid userId = Guid.Parse(userIdClaim.Value);
        var notifications = await _notificationRepository.GetNotificationsByReceiverIdAsync(userId);
        var noti = notifications.Select(n=>new NotificationDto
        {
            Id = n.Id,
            Title = n.Title,
            Message = n.Message,
            RedirectUrl = n.RedirectUrl,
            Type = n.Type,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt
        }).ToList();
        return Ok(ApiResponse<IEnumerable<NotificationDto>>.Ok(noti)); 
    }
    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadNotificationCount()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized("Not found user information.");
        Guid userId = Guid.Parse(userIdClaim.Value);
        int unreadCount = await _notificationRepository.CountUnreadNotificationsAsync(userId);
        return Ok(new { unreadCount });
    }
    [HttpPatch("{notiId}/read")]
    public async Task<IActionResult> MarkAsRead(Guid notiId)
    {
        var notification = await _notificationRepository.GetByIdAsync(notiId);
        if (notification == null) return NotFound();
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized("Not found user information.");
        Guid userId = Guid.Parse(userIdClaim.Value);
        if (notification.ReceiverId != userId) return Unauthorized();
        notification.IsRead = true;
        await _notificationRepository.UpdateAsync(notification);
        await _notificationRepository.SaveChangesAsync();
        return NoContent(); 
    }
}