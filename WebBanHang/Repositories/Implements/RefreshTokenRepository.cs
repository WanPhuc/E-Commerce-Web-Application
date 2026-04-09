using WebBanHang.Data;
using WebBanHang.Models.EntityModels;
using WebBanHang.Repositories.Interfaces;
using WebBanHang.Repositories.SqlServer;

namespace WebBanHang.Repositories.Implements
{
    public class RefreshTokenRepository:BaseRepository<RefreshToken>,IRefreshTokenRepository
    {
        private readonly AppDbContext _context;
        public RefreshTokenRepository(AppDbContext context): base(context) 
        {
            _context = context;
        }
    }
}
