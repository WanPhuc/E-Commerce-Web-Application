using WebBanHang.Data;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Repositories.Interfaces;
using WebBanHang.Models.EntityModels;
using WebBanHang.Repositories.SqlServer;
namespace WebBanHang.Repositories;

public class OderRepository : BaseRepository<Order>, IOderRepository
{
    public OderRepository(AppDbContext context): base(context){}
    
}