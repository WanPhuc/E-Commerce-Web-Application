using Microsoft.EntityFrameworkCore;
using WebBanHang.Models.EntityModels;
namespace WebBanHang.Repositories.Interfaces;

public interface ICartRepository : IRepository<Cart>
{
    Task<Cart?> GetByUserIdAsync(Guid userId);
    Task<Cart?> GetCartDetailsByUserIdAsync(Guid userId);
}