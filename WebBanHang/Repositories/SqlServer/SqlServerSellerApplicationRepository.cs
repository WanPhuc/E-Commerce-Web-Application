using Microsoft.EntityFrameworkCore;
using WebBanHang.Data;
using WebBanHang.Models;
using WebBanHang.Models.Enums;

namespace WebBanHang.Repositories.SqlServer;


public class SqlServerSellerApplicationRepository
    : SqlServerRepository<SellerApplication>, ISellerApplicationRepository
{
    public SqlServerSellerApplicationRepository(AppDbContext context) : base(context) { }

    
    protected override IQueryable<SellerApplication> Query =>
        _dbSet.Include(sa => sa.User);

    /// <summary> Lấy danh sách Pending </summary>
    public async Task<List<SellerApplication>> GetSellerApplicationsByStatusPendingAsync()
        => await Query.Where(sa => sa.Status ==  SellerApplicationStatus.Pending).ToListAsync();

    /// <summary> Lấy theo UserId </summary>
    public async Task<SellerApplication?> GetSellerApplicationByUserIdAsync(Guid userId)
        => await Query.FirstOrDefaultAsync(sa => sa.UserId == userId);

    /// <summary> Override GetByIdAsync để có include User </summary>
    public override async Task<SellerApplication?> GetByIdAsync(Guid id)
        => await Query.FirstOrDefaultAsync(sa => sa.Id == id);
}
