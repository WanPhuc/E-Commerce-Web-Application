using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;
using WebBanHang.Core.Models;
using WebBanHang.Data;
using WebBanHang.Repositories.Interfaces;


namespace WebBanHang.Repositories.SqlServer
{
    public class BaseRepository<T> : IBaseRepository<T> where T : Entity
    {
        private readonly AppDbContext _db;
        public BaseRepository(AppDbContext db)
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
            item = includeProperties.Aggregate(item,(current,includeProperties)=>current.Include(includeProperties));
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
            item = includeProperties.Aggregate(item,(current,includeProperties)=>current.Include(includeProperties));
            return item.ToListAsync();
        }
        public async Task<T?> FindSingleByConditionAsync(Expression<Func<T, bool>> expression, bool trackChanges = false)
        {
            return await GetQuery(trackChanges).Where(expression).FirstOrDefaultAsync();
        }
        public async Task<T?> FindSingleByConditionAsync(Expression<Func<T, bool>> expression, bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
        {
            var item = GetQuery(trackChanges).Where(expression);
            item = includeProperties.Aggregate(item,(current,includeProperties)=>current.Include(includeProperties));
            return await item.FirstOrDefaultAsync();
        }
        public async Task<int> CountByConditionAsync(Expression<Func<T, bool>> expression, bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
        {
            var item = GetQuery(trackChanges).Where(expression);
            item = includeProperties.Aggregate(item,(current,includeProperties)=>current.Include(includeProperties));
            return await item.CountAsync();
        }
        public async Task <T?> GetByIdAsync(Guid Id)
        {
            return await FindSingleByConditionAsync(x => x.Id == Id && !x.DeleteFlg);
        }
        public async Task<T?> GetByIdAsync(Guid Id, params Expression<Func<T, object>>[] includeProperties)
        {
            return await FindSingleByConditionAsync(x => x.Id == Id && !x.DeleteFlg, false, includeProperties);
        }
        public async Task<int> CreateAsync(T entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.DeleteFlg = false;
            await _db.Set<T>().AddAsync(entity);
            return await SaveChangesAsync();
        }
        public async Task<IList<Guid>> CreateListAsync(IEnumerable<T> entities)
        {
            foreach (var item in entities)
            {
                item.CreatedAt = DateTime.UtcNow;
                item.UpdatedAt = DateTime.UtcNow;
                item.DeleteFlg = false;
            }
            await _db.Set<T>().AddRangeAsync(entities);
            return entities.Select(x => x.Id).ToList();
        }
        public async Task<bool> UpdateAsync(T entity)
        {
            var exists = await _db.Set<T>().FindAsync(entity.Id);
            if (exists == null) return false;
            entity.UpdatedAt = DateTime.UtcNow;
            _db.Entry(exists).CurrentValues.SetValues(entity);
            return await SaveChangesAsync() > 0;
        }
        public async Task<bool> UpdateListAsync(IEnumerable<T> entities)
        {
            foreach(var item in entities)
            {
                item.UpdatedAt= DateTime.UtcNow;
            }
            _db.Set<T>().UpdateRange(entities);
            return await SaveChangesAsync() > 0;
        }
        public async Task<bool> SoftDeleteAsync(Guid Id)
        {
            var entity = await GetByIdAsync(Id);
            if(entity == null) return false;
            entity.UpdatedAt= DateTime.UtcNow;
            entity.DeleteFlg = true;

            await SaveChangesAsync();
            return true;
        }
        public async Task<bool> SoftDeleteListAsync(IEnumerable<Guid> ids)
        {
            var entity = await FindByConditionAsync(x => ids.Contains(x.Id) && !x.DeleteFlg);
            if( entity != null && entity.Count() > 0)
            {
                foreach (var enti in entity)
                {
                    enti.UpdatedAt = DateTime.UtcNow;
                    enti.DeleteFlg = true;
                }
                await SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool>HardDeleteAsync(Guid Id)
        {
            var entity = await GetByIdAsync(Id);
            if (entity == null) return false;
            _db.Set<T>().Remove(entity);
            await SaveChangesAsync();
            return true;
        }
        public async Task<bool>HardDeleteListAsync(IEnumerable<Guid> ids)
        {
            var entity = await FindByConditionAsync(x => ids.Contains(x.Id) && !x.DeleteFlg);
            if (entity != null && entity.Count() > 0)
            {
                _db.Set<T>().RemoveRange(entity);
                return true;
            }
            return false;

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
            await SaveChangesAsync();
            await _db.Database.CommitTransactionAsync();
        }
        public async Task RollbackTransactionAsync()
        {
            await _db.Database.RollbackTransactionAsync();
        }
    }
}
