using WebBanHang.Models.DTOs.Seller.Inventory;

namespace WebBanHang.Services.Seller.Interfaces;
public interface ISellerInventoryService
{
    Task<List<SellerInventoryDto>> GetSellerInventoryAsync(Guid userId,string? filter=null);
    Task UpdateSellerInventoryAsync(Guid productId,UpdateSellerInventoryDto updateDto);
}