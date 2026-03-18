using WebBanHang.Models;

public class AdminProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string SKU { get; set; } = default!;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public ProductStatus Status { get; set; }

    public Guid SellerId { get; set; }
    public string SellerName { get; set; } = default!;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = default!;
    public DateTime createdAt { get; set; }
}
public static class ProductMappong
{
    public static AdminProductDto ToAdminDto(this Product p) => new()
    {
        Id=p.Id,
        Name=p.Name,
        SKU=p.SKU,
        Price=p.Price,
        Stock=p.Stock,
        Status=p.Status,
        SellerId=p.SellerId,
        SellerName=p.Seller.StoreName,
        CategoryId=p.CategoryId,
        CategoryName=p.Category.Name,
        createdAt=p.CreatedAt
    
    };
}