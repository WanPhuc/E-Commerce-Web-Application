using WebBanHang.Models.DTOs.Seller.Order;
using WebBanHang.Models.Enums;

namespace WebBanHang.Services.Seller.Interfaces;
public interface ISellerOrderService
{
    Task<List<SellerOrderDto>> GetSellerOrdersAsync(Guid userId);
    Task<SellerOrderDto> GetDetailSellerOrderByIdAsync( Guid orderId,Guid userId);
    Task UpdateSellerOrderStatusAsync(Guid userId, Guid orderId, OrderStatus newStatus);
}