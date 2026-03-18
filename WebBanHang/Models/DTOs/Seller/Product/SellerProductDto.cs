namespace WebBanHang.Models.DTOs.Sellers.Product;
public class SellerProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; } = default!;
    public string SKU { get; set; } = default!;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public ProductStatus Status { get; set; }= ProductStatus.Active;
    public double DiscountPercent { get; set; }
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public int SoldCount { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = default!;
    public DateTime CreatedAt { get; set; }

    public string? MainImageUrl { get; set; }
    public List<ProductImageDto> Images {get; set;} = new ();

}
public class ProductImageDto
{
    public Guid Id { get; set; }
    public string ImageUrl { get; set; } = default!;
    public bool IsMainImage { get; set; }
}