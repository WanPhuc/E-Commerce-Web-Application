using Microsoft.EntityFrameworkCore;
using WebBanHang.Data;
using WebBanHang.Repositories.Interfaces;
using WebBanHang.Models.EntityModels;
using WebBanHang.Repositories.SqlServer;
namespace WebBanHang.Repositories;

public class CartRepository : BaseRepository<Cart>, ICartRepository
{
    public CartRepository(AppDbContext context): base(context){}
    
}