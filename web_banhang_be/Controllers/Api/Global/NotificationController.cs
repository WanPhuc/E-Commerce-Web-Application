using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Controllers.Api.Base;
using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Global;
using WebBanHang.Repositories.Interfaces;
using WebBanHang.Services.Global.Interfaces;

namespace WebBanHang.Controllers.Api.Global;

[ApiController]
[Route("api/v1/global/notifications")]
[Authorize]
public class NotificationController : BaseController
{
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationService _notificationService; 
    public NotificationController( INotificationRepository notificationRepository,INotificationService notificationService)
    {
        
        _notificationRepository = notificationRepository;
        _notificationService = notificationService;
    }
    [HttpGet]
    public async Task<IActionResult> GetMyNotifications([FromQuery] PagedRequest request)
    {
        var noti = await _notificationService.GetMyNotifications(request);
        return BaseResult(noti);
    }
    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadNotificationCount()
    {
        
        var unreadCount = await _notificationService.GetUnreadNotificationCount();
        return BaseResult(unreadCount);
    }
    [HttpPatch("{notiId}/read")]
    public async Task<IActionResult> MarkAsRead(Guid notiId)
    {
        var result = await _notificationService.MarkAsRead(notiId);
        return BaseResult(result);
    }
}
