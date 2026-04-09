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
                Permissions = UserPermission.Admin
            },
            new Role
            {
                Name = "Customer",
                Description ="Nguoi dung thuong",
                Permissions = UserPermission.User
            },
            new Role
            {
                Name = "Seller",
                Description ="Nguoi ban hang",
                Permissions = UserPermission.Seller
            }
        };
        context.Roles.AddRange(roles);
        await context.SaveChangesAsync();
    }
}
