using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using WebBanHang.Helpers;
using WebBanHang.Models.Enums;

namespace WebBanHang.Middleware;

public class AutoPermissionMiddleware
{
    private readonly RequestDelegate _next;

    public AutoPermissionMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context)
    {
        var endpoint = context.GetEndpoint();

        if (endpoint == null)
        {
            await _next(context);
            return;
        }

        if (endpoint.Metadata.GetMetadata<IAllowAnonymous>() != null)
        {
            await _next(context);
            return;
        }

        // ✅ Lấy method + route pattern (vd: "api/admin/orders/{id}")
        var method = context.Request.Method.ToUpperInvariant();
        var routePattern = (endpoint as RouteEndpoint)?.RoutePattern?.RawText;

        // ✅ Resolve permission theo API method + routePattern
        if (!ApiPermissionResolver.TryResolve(method, routePattern, out var requiredPerm)
            || requiredPerm == UserPermission.None)
        {
            await _next(context);
            return;
        }

        if (!(context.User.Identity?.IsAuthenticated ?? false))
        {
            await context.ChallengeAsync();
            return;
        }

        if (!PermissionHelper.Has(context, requiredPerm))
        {
            await context.ForbidAsync();
            return;
        }

        await _next(context);
    }
}
