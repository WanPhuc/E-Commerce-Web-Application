using WebBanHang.Models;
using Microsoft.EntityFrameworkCore;
namespace WebBanHang.Repositories;

public interface ICartRepository : IRepository<Cart>
{
    Task<Cart?> GetByUserIdAsync(Guid userId);
    Task<Cart?> GetCartDetailsByUserIdAsync(Guid userId);
}