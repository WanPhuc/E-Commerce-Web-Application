namespace WebBanHang.Models.DTOs.Seller.ProductImage;
public class ProductImageUpdateDto
{
    public string ImageUrl { get; set; } = default!;
    public bool IsMainImage { get; set; }
}