using WebBanHang.Models;
using WebBanHang.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByIdWithRoleSellerAsync(Guid id);
    Task<List<User>> GetAllUserAsync();
    Task<bool> ExistsByEmailAsync(string email);
}
