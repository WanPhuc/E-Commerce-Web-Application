using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebBanHang.Core.Models;
namespace WebBanHang.Models;
public class Product : Entity
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = default!;

    public string? Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Range(0,100)]
    public double DiscountPercent { get; set; }

    public int SoldCount { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    public int LowStockThreshold { get; set; }

    [Required]
    public string SKU { get; set; } = default!;

    public ProductStatus Status { get; set; } = ProductStatus.Pending;

    public Guid SellerId { get; set; }
    public Seller Seller { get; set; } = default!;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = default!;

    public ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();

    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
}
