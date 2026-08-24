using WebBanHang.Models.Enums;
using WebBanHang.Data;
using WebBanHang.Models.EntityModels;
namespace WebBanHang.Data.Seeders;

public static class RoleSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if(context.Roles.Any()) return;

        var roles = new List<Role>
        {
            new Role
            {
                Name = "Admin",
                Description ="Quan tri he thong",
                CreatedAt = DateTime.UtcNow,
            },
            new Role
            {
                Name = "Buyer",
                Description ="Nguoi dung thuong",
                CreatedAt = DateTime.UtcNow,
            },
            new Role
            {
                Name = "Seller",
                Description ="Nguoi ban hang",
                CreatedAt = DateTime.UtcNow,
            }
        };
        context.Roles.AddRange(roles);
        await context.SaveChangesAsync();
    }
}
