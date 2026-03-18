using WebBanHang.Models;

namespace WebBanHang.Repositories;
public interface ISellerApplicationRepository : IRepository<SellerApplication>
{
    Task<List<SellerApplication>> GetSellerApplicationsByStatusPendingAsync();

    Task<SellerApplication?> GetSellerApplicationByUserIdAsync(Guid userId);
}
