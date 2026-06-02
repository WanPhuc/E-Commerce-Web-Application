using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Seller.Inventory;

namespace WebBanHang.Services.Seller.Interfaces;
public interface ISellerInventoryService
{
    Task<ApiResponse<PagedResult<SellerInventoryDto>>> GetSellerInventoryAsync(PagedRequest request, string? filter=null);
    Task<ApiResponse<string>> UpdateSellerInventoryAsync(Guid productId,UpdateSellerInventoryDto updateDto);
}
