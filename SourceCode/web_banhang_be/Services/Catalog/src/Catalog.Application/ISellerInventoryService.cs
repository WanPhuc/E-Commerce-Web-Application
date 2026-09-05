
using AuraMart.Catalog.Dtos;
namespace AuraMart.Catalog.Services;
public interface ISellerInventoryService
{
    Task<ApiResponse<PagedResult<SellerInventoryDto>>> GetSellerInventoryAsync(PagedRequest request, string? filter=null);
    Task<ApiResponse<string>> UpdateSellerInventoryAsync(Guid productId,UpdateSellerInventoryDto updateDto);
}
