namespace WebBanHang.Models.DTOs.Seller.ProductImage;
public class ProductImageCreateDto
{
    public string ImageUrl { get; set; } = default!;
    public bool IsMainImage { get; set; }
}
