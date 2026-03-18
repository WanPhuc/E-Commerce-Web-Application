using Microsoft.EntityFrameworkCore;
using WebBanHang.Data;
using WebBanHang.Models;
using WebBanHang.Repositories;

public class SqlServerRoleRepository : SqlServerRepository<Role>,IRoleRepository
{
    public SqlServerRoleRepository(AppDbContext context) : base(context) { }
    protected override IQueryable<Role> Query => _dbSet;
    public async Task<Role?> GetByNameAsync(string name)
    {
        return await Query.FirstOrDefaultAsync(r => r.Name == name);
    }
}