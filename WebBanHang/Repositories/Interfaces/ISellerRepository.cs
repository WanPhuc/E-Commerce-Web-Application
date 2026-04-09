using WebBanHang.Models.EntityModels;
namespace WebBanHang.Repositories.Interfaces;
public interface ISellerRepository : IRepository<Seller>
{
    Task<Seller?> GetSellerByUserIdAsync(Guid userId);
}