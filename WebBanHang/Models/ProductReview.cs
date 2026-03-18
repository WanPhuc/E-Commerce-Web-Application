using System.ComponentModel.DataAnnotations;
using WebBanHang.Core.Models;

namespace WebBanHang.Models;
public class ProductReview:Entity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;
    [Range(1,5)]
    public int Rating { get; set; }
    [MaxLength(500)]
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [MaxLength(500)]
    public string? SellerReply { get; set; }
    public DateTime? SellerReplyAt { get; set; }
}
