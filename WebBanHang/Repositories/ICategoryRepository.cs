using WebBanHang.Models;
using WebBanHang.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    Task<List<Category>> GetAllWithChildrenAsync(CancellationToken ct = default);
    Task<Category?> GetByIdWithChildrenAsync(Guid id, CancellationToken ct = default);

    Task<bool> ExistsByNameAsync(string name, Guid? parentId, CancellationToken ct = default);
    Task<bool> ExistsByNameExceptAsync(string name, Guid? parentId,Guid exceptId, CancellationToken ct = default);
    Task<bool> HasChildrenAsync(Guid id, CancellationToken ct = default);
}
