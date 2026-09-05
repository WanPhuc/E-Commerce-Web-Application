using System.ComponentModel.DataAnnotations;

namespace AuraMart.Catalog.Domain;
public class ProductReview:Entity
{
    public Guid UserId { get; set; }

    // Snapshot ten nguoi dung luc review (FK da cat - Phase 2)
    [MaxLength(50)]
    public string? DisplayName { get; set; }

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;
    [Range(1,5)]
    public int Rating { get; set; }
    [MaxLength(500)]
    public string? Comment { get; set; }
    [MaxLength(500)]
    public string? SellerReply { get; set; }
    public DateTime? SellerReplyAt { get; set; }
}
