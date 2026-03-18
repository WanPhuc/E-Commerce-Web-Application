using WebBanHang.Models;
using WebBanHang.Data;
using Microsoft.EntityFrameworkCore;
namespace WebBanHang.Repositories;

public class SqlServerOderRepository : SqlServerRepository<Order>, IOderRepository
{
    public SqlServerOderRepository(AppDbContext context): base(context){}
    protected override IQueryable<Order> Query => _dbSet
        .Include(o=>o.User)
        .Include(o=>o.Address)
        .Include(o=>o.Payment)
        .Include(o=>o.Items)
            .ThenInclude(oi=>oi.Product)
                .ThenInclude(p=>p.Images)
        .AsNoTracking();

    public override async Task<Order?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }
    public async Task<Order?> GetOrderDetailsByIdAsync(Guid id)
    {
        return await Query.FirstOrDefaultAsync(o=>o.Id==id);
    }
    public async Task<List<Order>> GetOrdersByUserAsync(Guid userId)
    {
        return await Query.Where(o=>o.UserId==userId).OrderByDescending(o=>o.CreatedAt).ToListAsync();
    }
    public async Task<List<Order>> GetOrdersBySellerAsync(Guid sellerId)
    {
        return await Query.Where(o=>o.SellerId==sellerId).OrderByDescending(o=>o.CreatedAt).ToListAsync();
    }
}