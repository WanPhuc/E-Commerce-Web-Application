using Microsoft.EntityFrameworkCore;
using WebBanHang.Helpers;
using WebBanHang.Models;
namespace WebBanHang.Data.Seeders;
public static class UserSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (context.Users.Any()) return;

        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
        if(adminRole == null)  return;
        var admin=new User
        {
            FullName="admin",
            Email="admin@gmail.com",
            PasswordHash=PasswordHelper.HashPassword("admin123"),
            CreatedAt=DateTime.Now,
            IsActive=true,
            RoleId=adminRole.Id
        };
        context.Users.Add(admin);
        await context.SaveChangesAsync();
    }
}