using WebBanHang.Models.EntityModels;

namespace WebBanHang.Repositories.Interfaces;
public interface ISellerApplicationRepository : IRepository<SellerApplication>
{
    Task<List<SellerApplication>> GetSellerApplicationsByStatusPendingAsync();

    Task<SellerApplication?> GetSellerApplicationByUserIdAsync(Guid userId);
}
