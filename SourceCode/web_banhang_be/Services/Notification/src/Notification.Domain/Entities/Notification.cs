using System.ComponentModel.DataAnnotations;
using VanFucVN.Core.Persistence.Domain;

namespace AuraMart.Notification.Domain;

public enum NotificationType
{
    General = 0,
    Order = 1,
    Promotion = 2,
    System = 3
}

public class Notification : Entity
{
    [Required]
    public Guid ReceiverId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Message { get; set; } = string.Empty;

    public string? RedirectUrl { get; set; }

    public NotificationType Type { get; set; } = NotificationType.General;

    public bool IsRead { get; set; }
}
