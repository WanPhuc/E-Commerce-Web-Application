using Microsoft.EntityFrameworkCore;
using WebBanHang.Data;
using WebBanHang.Models;
using WebBanHang.Repositories;

public class SqlServerCategoryRepository : SqlServerRepository<Category>, ICategoryRepository
{
    public SqlServerCategoryRepository(AppDbContext context) : base(context) { }
    protected override IQueryable<Category> Query => _dbSet
        .AsNoTracking()
        .Include(c => c.SubCategories);

    public async Task<List<Category>> GetAllWithChildrenAsync(CancellationToken ct = default)
    {
        return await Query.ToListAsync(ct);
    }
    public async Task<Category?> GetByIdWithChildrenAsync(Guid id,CancellationToken ct = default)
    {
        return await Query.FirstOrDefaultAsync(c=>c.Id == id,ct);
    }
    public async Task<bool> ExistsByNameAsync(string name, Guid? parentId, CancellationToken ct = default)
    {
        name = name.Trim();
        return await _dbSet.AnyAsync(c => c.ParentId == parentId && c.Name == name, ct);
    }

    public async Task<bool> HasChildrenAsync(Guid id,CancellationToken ct = default)
    {
        return await _dbSet.AnyAsync(c=>c.ParentId == id,ct);
    }
    public Task<bool> ExistsByNameExceptAsync(string name, Guid? parentId, Guid exceptId, CancellationToken ct = default)
    {
        name = name.Trim();
        return _dbSet.AnyAsync(c =>
            c.Id != exceptId &&
            c.ParentId == parentId &&
            c.Name == name,
            ct);
    }

}