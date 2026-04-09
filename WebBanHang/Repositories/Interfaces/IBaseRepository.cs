using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;
using WebBanHang.Core.Models;

namespace WebBanHang.Repositories.Interfaces
{
    public interface IBaseRepository<T> where T : Entity
    {
        Task<List<T>> GetAllAsync(bool trackChanges = false);
        Task<List<T>> GetAllAsync(bool trackChanges = false,params Expression<Func< T, object>>[] includeProperties);
        Task<bool> AnyByConditionAsync(Expression<Func<T,bool>> expression);
        Task<List<T>> FindByConditionAsync(Expression<Func<T,bool>> expression, bool trackChanges = false);
        Task<List<T>> FindByConditionAsync(Expression<Func<T,bool>> expression, bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties);
        Task<T?> FindSingleByConditionAsync(Expression<Func<T,bool>> expression, bool trackChanges = false);
        Task<T?> FindSingleByConditionAsync(Expression<Func<T,bool>> expression, bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties);
        Task<int> CountByConditionAsync(Expression<Func<T,bool>> expression,bool trackChanges = false,params Expression<Func<T, object>>[] includeProperties);
        Task<T?> GetByIdAsync(Guid Id);
        Task<T?> GetByIdAsync(Guid Id,params Expression<Func<T, object>>[] includeProperties);
        Task<int> CreateAsync(T entity);
        Task<IList<Guid>> CreateListAsync(IEnumerable<T> entities);
        Task<bool> UpdateAsync(T entity);
        Task<bool> UpdateListAsync(IEnumerable<T> entities);
        Task<bool> SoftDeleteAsync(Guid id);   
        Task<bool> SoftDeleteListAsync(IEnumerable<Guid> ids);
        Task<bool> HardDeleteAsync(Guid id);
        Task<bool> HardDeleteListAsync(IEnumerable<Guid> ids);
        Task<int> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task EndTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
