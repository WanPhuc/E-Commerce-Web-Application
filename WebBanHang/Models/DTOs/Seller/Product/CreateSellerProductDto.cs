using System.ComponentModel.DataAnnotations;
using WebBanHang.Models.DTOs.Seller.ProductImage;

namespace WebBanHang.Models.DTOs.Sellers.Product;
public class CreateSellerProductDto
{
    [Required]
    public string Name { get; set; } = default!;
    public string? Description { get; set; } = default!;
    [Required]
    public string SKU { get; set; } = default!;
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public ProductStatus Status { get; set; }= ProductStatus.Active;
    public double DiscountPercent { get; set; }
    public Guid CategoryId { get; set; }
    public List<ProductImageCreateDto>? Images {get; set;}
}