namespace WebBanHang.Models.DTOs.Global;
public class NotificationDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string? RedirectUrl { get; set; }
    public NotificationType Type { get; set; } 
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}