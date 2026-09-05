using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;
using VanFucVN.Core.Persistence.Domain;

namespace VanFucVN.Core.Persistence.Repositories;

public class BaseRepository<T, TContext> : IBaseRepository<T>
    where T : Entity
    where TContext : DbContext
{
    protected readonly TContext _db;

    public BaseRepository(TContext db)
    {
        _db = db;
    }

    protected IQueryable<T> GetQuery(bool trackChanges) => trackChanges ? _db.Set<T>() : _db.Set<T>().AsNoTracking();

    public async Task<List<T>> GetAllAsync(bool trackChanges = false)
    {
        return await GetQuery(trackChanges).ToListAsync();
    }

    public async Task<List<T>> GetAllAsync(bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
    {
        var item = GetQuery(trackChanges);
        item = includeProperties.Aggregate(item, (current, includeProperty) => current.Include(includeProperty));
        return await item.ToListAsync();
    }

    public async Task<bool> AnyByConditionAsync(Expression<Func<T, bool>> expression)
    {
        return await _db.Set<T>().AsNoTracking().AnyAsync(expression);
    }

    public async Task<List<T>> FindByConditionAsync(Expression<Func<T, bool>> expression, bool trackChanges = false)
    {
        return await GetQuery(trackChanges).Where(expression).ToListAsync();
    }

    public Task<List<T>> FindByConditionAsync(Expression<Func<T, bool>> expression, bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
    {
        var item = GetQuery(trackChanges).Where(expression);
        item = includeProperties.Aggregate(item, (current, includeProperty) => current.Include(includeProperty));
        return item.ToListAsync();
    }

    public async Task<T?> FindSingleByConditionAsync(Expression<Func<T, bool>> expression, bool trackChanges = false)
    {
        return await GetQuery(trackChanges).Where(expression).FirstOrDefaultAsync();
    }

    public async Task<T?> FindSingleByConditionAsync(Expression<Func<T, bool>> expression, bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
    {
        var item = GetQuery(trackChanges).Where(expression);
        item = includeProperties.Aggregate(item, (current, includeProperty) => current.Include(includeProperty));
        return await item.FirstOrDefaultAsync();
    }

    public async Task<int> CountByConditionAsync(Expression<Func<T, bool>> expression, bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
    {
        var item = GetQuery(trackChanges).Where(expression);
        item = includeProperties.Aggregate(item, (current, includeProperty) => current.Include(includeProperty));
        return await item.CountAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _db.Set<T>().FindAsync(id);
    }

    public async Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includeProperties)
    {
        var item = GetQuery(false);
        item = includeProperties.Aggregate(item, (current, includeProperty) => current.Include(includeProperty));
        return await item.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<int> CreateAsync(T entity)
    {
        await _db.Set<T>().AddAsync(entity);
        return await _db.SaveChangesAsync();
    }

    public async Task<IList<Guid>> CreateListAsync(IEnumerable<T> entities)
    {
        var list = entities.ToList();
        await _db.Set<T>().AddRangeAsync(list);
        await _db.SaveChangesAsync();
        return list.Select(e => e.Id).ToList();
    }

    public async Task<bool> UpdateAsync(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _db.Set<T>().Update(entity);
        return await _db.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateListAsync(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
        {
            entity.UpdatedAt = DateTime.UtcNow;
        }
        _db.Set<T>().UpdateRange(entities);
        return await _db.SaveChangesAsync() > 0;
    }

    public async Task<bool> SoftDeleteAsync(Guid id)
    {
        var entity = await _db.Set<T>().FindAsync(id);
        if (entity == null) return false;
        entity.DeleteFlg = true;
        entity.UpdatedAt = DateTime.UtcNow;
        return await _db.SaveChangesAsync() > 0;
    }

    public async Task<bool> SoftDeleteListAsync(IEnumerable<Guid> ids)
    {
        var entities = await _db.Set<T>().Where(x => ids.Contains(x.Id)).ToListAsync();
        if (entities.Count == 0) return false;
        foreach (var entity in entities)
        {
            entity.DeleteFlg = true;
            entity.UpdatedAt = DateTime.UtcNow;
        }
        return await _db.SaveChangesAsync() > 0;
    }

    public async Task<bool> HardDeleteAsync(Guid id)
    {
        var entity = await _db.Set<T>().FindAsync(id);
        if (entity == null) return false;
        _db.Set<T>().Remove(entity);
        return await _db.SaveChangesAsync() > 0;
    }

    public async Task<bool> HardDeleteListAsync(IEnumerable<Guid> ids)
    {
        var entities = await _db.Set<T>().Where(x => ids.Contains(x.Id)).ToListAsync();
        if (entities.Count == 0) return false;
        _db.Set<T>().RemoveRange(entities);
        return await _db.SaveChangesAsync() > 0;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _db.SaveChangesAsync();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _db.Database.BeginTransactionAsync();
    }

    public async Task EndTransactionAsync()
    {
        await _db.Database.CommitTransactionAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        await _db.Database.RollbackTransactionAsync();
    }

    // ============ IQueryable ============
    public IQueryable<T> GetAll(bool trackChanges = false)
    {
        return GetQuery(trackChanges);
    }

    public IQueryable<T> GetAll(bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
    {
        var item = GetQuery(trackChanges);
        return includeProperties.Aggregate(item, (current, includeProperty) => current.Include(includeProperty));
    }

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false)
    {
        return GetQuery(trackChanges).Where(expression);
    }

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
    {
        var item = GetQuery(trackChanges).Where(expression);
        return includeProperties.Aggregate(item, (current, includeProperty) => current.Include(includeProperty));
    }
}
