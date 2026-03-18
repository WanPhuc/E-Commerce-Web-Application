namespace WebBanHang.Models.DTOs.Seller.Product;
public class UpdateSellerProductDto
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; } = default!;
    public string SKU { get; set; } = default!;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public ProductStatus Status { get; set; }
    public double DiscountPercent { get; set; }
    public Guid CategoryId { get; set; }
}