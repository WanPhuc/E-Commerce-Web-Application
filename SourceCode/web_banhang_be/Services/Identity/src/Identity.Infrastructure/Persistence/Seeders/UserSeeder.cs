using Microsoft.EntityFrameworkCore;
using AuraMart.Shared.Constants;
using AuraMart.Identity.Application;

namespace WebBanHang.Data.Seeders;

public static class UserSeeder
{
    public static async Task SeedAsync(IdentityDbContext context)
    {
        if (await context.Users.AnyAsync(u => u.Id == DemoIds.AdminUserId || u.Email == DemoIds.AdminEmail))
            return;

        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
        if (adminRole == null) return;

        context.Users.Add(new User
        {
            Id = DemoIds.AdminUserId,
            FullName = "admin",
            Email = DemoIds.AdminEmail,
            PasswordHash = PasswordHelper.HashPassword(DemoIds.AdminPassword),
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            RoleId = adminRole.Id
        });
        await context.SaveChangesAsync();
    }
}
