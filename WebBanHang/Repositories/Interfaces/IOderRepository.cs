using WebBanHang.Models.EntityModels;

namespace WebBanHang.Repositories.Interfaces;
public interface IOderRepository : IRepository<Order>
{
    Task<Order?> GetOrderDetailsByIdAsync(Guid id);
    Task<List<Order>> GetOrdersByUserAsync(Guid userId);
    Task<List<Order>> GetOrdersBySellerAsync(Guid sellerId);
}