using WebBanHang.Models;
namespace WebBanHang.Repositories;
public interface ISellerRepository : IRepository<Seller>
{
    Task<Seller?> GetSellerByUserIdAsync(Guid userId);
}