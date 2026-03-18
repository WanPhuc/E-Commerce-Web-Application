namespace WebBanHang.Models.DTOs.Seller.Inventory;
public class SellerInventoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }= default!;
    public string SKU { get; set; }= default!;
    public int Stock { get; set; }
    public int LowStockThreshold { get; set; }
    public ProductStatus Status {get; set;}
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }

    public bool IsLowStock => Stock>0 && Stock <=LowStockThreshold;
    public bool IsOutOfStock => Stock == 0;
}

public class UpdateSellerInventoryDto
{
    public int? Stock { get; set; }
    public int? LowStockThreshold { get; set; }
    public ProductStatus? Status {get; set;}
}