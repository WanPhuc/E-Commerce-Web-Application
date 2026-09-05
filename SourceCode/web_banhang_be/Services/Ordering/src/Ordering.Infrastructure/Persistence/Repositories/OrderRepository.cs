using Microsoft.EntityFrameworkCore;

namespace AuraMart.Ordering.Infrastructure.Persistence.Repositories;

public class OrderRepository : BaseRepository<Order, OrderingDbContext>, IOrderRepository
{
    public OrderRepository(OrderingDbContext context) : base(context) { }

    public async Task<Order?> GetOrderWithItemsAsync(Guid orderId)
    {
        return await _db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId && !o.DeleteFlg);
    }

    public async Task<List<Order>> GetOrdersByUserIdAsync(Guid userId)
    {
        return await _db.Orders
            .Include(o => o.Items)
            .Where(o => o.UserId == userId && !o.DeleteFlg)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Order>> GetOrdersBySellerIdAsync(Guid sellerId)
    {
        return await _db.Orders
            .Include(o => o.Items)
            .Where(o => o.SellerId == sellerId && !o.DeleteFlg)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }
}
