using Microsoft.EntityFrameworkCore;
using WebBanHang.Data;
using WebBanHang.Models.EntityModels;
using WebBanHang.Repositories;
using WebBanHang.Repositories.SqlServer;

public class RoleRepository : BaseRepository<Role>,IRoleRepository
{
    private readonly AppDbContext _context;
    public RoleRepository(AppDbContext context) : base(context) { 
        _context = context;
    }
   
}