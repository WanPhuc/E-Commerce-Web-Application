using WebBanHang.Data;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Repositories.Interfaces;
using WebBanHang.Models.EntityModels;
using WebBanHang.Repositories.SqlServer;
namespace WebBanHang.Repositories;
public class SqlServerSellerRepository : BaseRepository<Seller>, ISellerRepository
{
    public SqlServerSellerRepository(AppDbContext context): base(context){}
    

}