using Microsoft.EntityFrameworkCore;
using WebBanHang.Data;
using WebBanHang.Models.EntityModels;
using WebBanHang.Models.Enums;
using WebBanHang.Repositories.Interfaces;

namespace WebBanHang.Repositories.SqlServer;


public class SellerApplicationRepository: BaseRepository<SellerApplication>, ISellerApplicationRepository
{
    public SellerApplicationRepository(AppDbContext context) : base(context) { }  
}
