using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Data;
using WebBanHang.Models.Enums;

namespace WebBanHang.Middleware;

public class PermissionMiddleware
{
    private readonly RequestDelegate _next;

    public PermissionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, AppDbContext db)
    {
        // Chưa login → bỏ qua
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            await _next(context);
            return;
        }

        // Lấy UserId từ Claims
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            await _next(context);
            return;
        }

        // Lấy user + role từ DB
        var user = await db.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id.ToString() == userId);

        if (user == null || user.Role == null)
        {
            await _next(context);
            return;
        }

        // Gán Permission vào HttpContext
        context.Items["Permission"] = user.Role.Permissions;

        await _next(context);
    }
}
