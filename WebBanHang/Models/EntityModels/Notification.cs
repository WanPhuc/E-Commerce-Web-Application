using WebBanHang.Core.Models;

namespace WebBanHang.Models.EntityModels;
public class Notification:Entity
{
    public Guid ReceiverId { get; set; }
    public string Title { get; set; } = default!;
    public string Message { get; set; } = default!;
    public string? RedirectUrl { get; set; }
    public bool IsRead { get; set; } = false;
    public NotificationType Type { get; set; }
}