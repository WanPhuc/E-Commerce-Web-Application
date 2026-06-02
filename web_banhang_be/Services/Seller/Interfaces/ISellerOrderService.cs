using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Seller.Order;
using WebBanHang.Models.Enums;

namespace WebBanHang.Services.Seller.Interfaces;
public interface ISellerOrderService
{
    Task<ApiResponse<PagedResult<SellerOrderDto>>> GetSellerOrdersAsync(PagedRequest request);
    Task<ApiResponse<SellerOrderDto>> GetDetailSellerOrderByIdAsync( Guid orderId );
    Task<ApiResponse<string>> UpdateSellerOrderStatusAsync( Guid orderId, OrderStatus newStatus);
}
