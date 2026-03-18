using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebBanHang.Data;
namespace WebBanHang.Repositories;

public class SqlServerRepository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public SqlServerRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }
    protected virtual IQueryable<T> Query => _dbSet.AsQueryable();

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }
    public virtual async Task<List<T>> GetAllAsync()
    {
        return await Query.ToListAsync();
    }
    public virtual async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await Query.Where(predicate).ToListAsync();
    }
    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
    public virtual async Task<int> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return await _context.SaveChangesAsync();
    }
    public virtual async Task<int> DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        return await _context.SaveChangesAsync();
    }
    public virtual async Task<int> DeleteByIdAsync(Guid id)
    {
        var entity =await  _dbSet.FindAsync(id);
        if (entity == null)
        {
            return 0;
        }
        _dbSet.Remove(entity);
        return await _context.SaveChangesAsync();
    }

    public virtual async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

}