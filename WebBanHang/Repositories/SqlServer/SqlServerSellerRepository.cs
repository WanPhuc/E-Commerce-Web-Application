using WebBanHang.Models;
using WebBanHang.Data;
using Microsoft.EntityFrameworkCore;
namespace WebBanHang.Repositories;
public class SqlServerSellerRepository : SqlServerRepository<Seller>, ISellerRepository
{
    public SqlServerSellerRepository(AppDbContext context): base(context){}
    protected override IQueryable<Seller> Query => _dbSet
        .Include(s=>s.User)
        .Include(s=>s.Address)
        .Include(s=>s.Products);
    public async Task<Seller?> GetSellerByUserIdAsync(Guid userId)
    {
        return await Query.FirstOrDefaultAsync(s=>s.UserId==userId);
    }
    public override async Task<Seller?> GetByIdAsync(Guid id)
    {
        return await Query.FirstOrDefaultAsync(s=>s.Id==id);
    }

}