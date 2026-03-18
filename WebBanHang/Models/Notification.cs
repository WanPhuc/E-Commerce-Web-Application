using WebBanHang.Core.Models;

namespace WebBanHang.Models;
public class Notification:Entity
{
    public Guid ReceiverId { get; set; }
    public string Title { get; set; } = default!;
    public string Message { get; set; } = default!;
    public string? RedirectUrl { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public NotificationType Type { get; set; }
}