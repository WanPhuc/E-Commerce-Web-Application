using Microsoft.EntityFrameworkCore;
using WebBanHang.Data;
using WebBanHang.Models;
using WebBanHang.Repositories;

public class SqlServerUserRepository : SqlServerRepository<User>, IUserRepository
{
    public SqlServerUserRepository(AppDbContext context) : base(context) { }

    protected override IQueryable<User> Query =>  _dbSet
        .Include(u=>u.Role)
        .Include(u=>u.Seller);

    public async Task<User?> GetByIdWithRoleSellerAsync(Guid id)
    {
        return await Query.FirstOrDefaultAsync(u => u.Id == id);
    }
    public async Task<List<User>> GetAllUserAsync()
    {
        return await Query.ToListAsync();
    }
    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email);
    }   
}