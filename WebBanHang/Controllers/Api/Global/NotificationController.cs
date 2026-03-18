using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Repositories;

namespace WebBanHang.Controllers.Api.Global;

[ApiController]
[Route("api/global/notifications")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationRepository _notificationRepository;
    public NotificationController( INotificationRepository notificationRepository)
    {
        
        _notificationRepository = notificationRepository;
    }
    [HttpGet]
    public async Task<IActionResult> GetMyNotifications()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized("Not found user information.");
        Guid userId = Guid.Parse(userIdClaim.Value);
        var notifications = await _notificationRepository.GetNotificationsByReceiverIdAsync(userId);

        return Ok(notifications.Select(n => new
        {
            id = n.Id,
            title = n.Title,
            message = n.Message,
            redirectUrl = n.RedirectUrl,
            type = n.Type.ToString(),
            isRead = n.IsRead,
            createdAt = n.CreatedAt,
        }));
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