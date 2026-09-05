namespace AuraMart.Ordering.Domain.Repositories;

public interface IOrderRepository : IBaseRepository<Order>
{
    Task<Order?> GetOrderWithItemsAsync(Guid orderId);
    Task<List<Order>> GetOrdersByUserIdAsync(Guid userId);
    Task<List<Order>> GetOrdersBySellerIdAsync(Guid sellerId);
}
