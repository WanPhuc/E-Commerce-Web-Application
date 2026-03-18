using WebBanHang.Models;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Data;
namespace WebBanHang.Repositories;

public class SqlServerCartRepository : SqlServerRepository<Cart>, ICartRepository
{
    public SqlServerCartRepository(AppDbContext context): base(context){}
    protected override IQueryable<Cart> Query => _dbSet
        .Include(c=>c.CartItems)
            .ThenInclude(ci=>ci.Product)
                .ThenInclude(p=>p.Images);

    public async Task<Cart?> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet.FirstOrDefaultAsync(c=>c.UserId==userId);
    }
    public async Task<Cart?> GetCartDetailsByUserIdAsync(Guid userId)
    {
        return await Query.FirstOrDefaultAsync(c=>c.UserId==userId);
    }

    public override async Task<Cart?> GetByIdAsync(Guid id)
    {
        return await Query.FirstOrDefaultAsync(c=>c.Id==id);
    }
}