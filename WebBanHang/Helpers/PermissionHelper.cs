using WebBanHang.Models.Enums;

namespace WebBanHang.Helpers;

public static class PermissionHelper
{
    public static bool Has(HttpContext ctx, UserPermission permission)
    {
        if (!ctx.Items.ContainsKey("Permission"))
            return false;

        var userPermission = (UserPermission)ctx.Items["Permission"];
        return userPermission.HasFlag(permission);
    }
}
