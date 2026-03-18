using WebBanHang.Models.Enums;

namespace WebBanHang.Helpers;

public static class ApiPermissionResolver
{
    /// <summary>
    /// Resolve permission theo API route + HTTP method.
    /// routePattern nên lấy từ RouteEndpoint.RoutePattern.RawText, vd: "api/admin/orders/{id}"
    /// </summary>
    public static bool TryResolve(
        string method,
        string? routePattern,
        out UserPermission permission)
    {
        method = method.ToUpperInvariant();
        var route = Normalize(routePattern);

        permission = UserPermission.None;

        // Không phải API -> bỏ qua
        if (route is null || !route.StartsWith("api/"))
            return false;

        // Tách segments: api/{scope}/{resource}/...
        // vd: api/admin/orders/{id}
        var seg = route.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (seg.Length < 3) return false;

        var scope = seg[1];     // admin | seller | user | public...
        var resource = seg[2];  // orders | products | categories ...

        // ========== 1) BỎ QUA PUBLIC / AUTH ==========
        // vd: api/auth/login, api/auth/register...
        if (scope is "auth" or "public")
            return false;

        // ========== 2) AREA MASK (ROLE TAG) ==========
        UserPermission areaMask = scope switch
        {
            "admin"  => UserPermission.AdminUser,
            "seller" => UserPermission.SellerUser,
            _        => UserPermission.None
        };

        // ========== 3) MODULE PERMISSION ==========
        UserPermission modulePermission = UserPermission.None;

        // DASHBOARD (vd: GET api/admin/dashboard)
        if (resource == "dashboard")
        {
            if (scope == "admin")
            {
                modulePermission = UserPermission.ViewAdminDashboard;
            }
            else if (scope == "seller")
            {
                modulePermission = UserPermission.ViewSellerDashboard;
            }
        }
        // ADDRESS: api/{scope}/addresses
        else if (resource is "address" or "addresses")
        {
            TryResolveCrudByMethod(
                method,
                view:   UserPermission.ViewAddresses,
                create: UserPermission.CreateAddress,
                edit:   UserPermission.EditAddress,
                delete: UserPermission.DeleteAddress,
                out modulePermission
            );
        }
        // CART: api/{scope}/cart(s)
        else if (resource is "cart" or "carts")
        {
            TryResolveCrudByMethod(
                method,
                view:   UserPermission.ViewCart,
                create: UserPermission.AddToCart,
                edit:   UserPermission.UpdateCart,
                delete: UserPermission.RemoveFromCart,
                out modulePermission
            );
        }
        // CATEGORY: api/{scope}/categories
        else if (resource is "category" or "categories")
        {
            TryResolveCrudByMethod(
                method,
                view:   UserPermission.ViewCategories,
                create: UserPermission.CreateCategory,
                edit:   UserPermission.EditCategory,
                delete: UserPermission.DeleteCategory,
                out modulePermission
            );
        }
        // ORDER: api/{scope}/orders
        else if (resource is "order" or "orders")
        {
            // Với API thường:
            // GET    -> ViewOrders
            // POST   -> CreateOrder (checkout)
            // PUT/PATCH -> UpdateOrder (update status)
            // DELETE -> CancelOrder (hoặc DeleteOrder nếu bạn tách)
            modulePermission = method switch
            {
                "GET" => UserPermission.ViewOrders,
                "POST" => UserPermission.CreateOrder,
                "PUT" or "PATCH" => UserPermission.UpdateOrder,
                "DELETE" => UserPermission.CancelOrder,
                _ => UserPermission.None
            };
        }
        // PAYMENT: api/{scope}/payments
        else if (resource is "payment" or "payments")
        {
            // Nếu refund là DELETE, còn view là GET
            modulePermission = method switch
            {
                "GET" => UserPermission.ViewPayments,
                "DELETE" => UserPermission.RefundPayment,
                _ => UserPermission.None
            };
        }
        // PRODUCT: api/{scope}/products
        else if (resource is "product" or "products")
        {
            TryResolveCrudByMethod(
                method,
                view:   UserPermission.ViewProducts,
                create: UserPermission.CreateProduct,
                edit:   UserPermission.EditProduct,
                delete: UserPermission.DeleteProduct,
                out modulePermission
            );
        }
        // SELLER: api/admin/sellers hoặc api/seller/profile
        else if (resource is "seller" or "sellers")
        {
            // Tuỳ bạn thiết kế. Đây là default:
            modulePermission = method switch
            {
                "GET" => UserPermission.ViewSeller,
                "PUT" or "PATCH" => UserPermission.EditSeller,
                _ => UserPermission.None
            };
        }
        // SELLER APPLICATION: api/admin/seller-applications
        else if (resource is "sellerapplication" or "sellerapplications" or "seller-applications")
        {
            // Gợi ý:
            // seller tạo đơn -> POST (CreateSellerApplication)
            // admin xem -> GET (ViewSellerApplications)
            // admin duyệt -> PUT/PATCH (ReviewSellerApplications)
            modulePermission = method switch
            {
                "POST" => UserPermission.CreateSellerApplication,
                "GET" => UserPermission.ViewSellerApplications,
                "PUT" or "PATCH" => UserPermission.ReviewSellerApplications,
                _ => UserPermission.None
            };
        }
        // USER: api/admin/users
        else if (resource is "user" or "users")
        {
            TryResolveCrudByMethod(
                method,
                view:   UserPermission.ViewUsers,
                create: UserPermission.CreateUser,
                edit:   UserPermission.EditUser,
                delete: UserPermission.DeleteUser,
                out modulePermission
            );
        }
        // ROLE: api/admin/roles
        else if (resource is "role" or "roles")
        {
            TryResolveCrudByMethod(
                method,
                view:   UserPermission.ViewRoles,
                create: UserPermission.CreateRole,
                edit:   UserPermission.EditRole,
                delete: UserPermission.DeleteRole,
                out modulePermission
            );
        }

        // ========== 4) DEFAULT DENY cho ADMIN/SELLER ==========
        // Nếu thuộc admin/seller mà modulePermission vẫn None => DENY
        if (areaMask != UserPermission.None && modulePermission == UserPermission.None)
        {
            permission = UserPermission.None;
            return true; // middleware sẽ chặn vì requiredPerm == None (theo logic của bạn)
        }

        // Nếu route public/khác scope, và không map module => bỏ qua
        if (areaMask == UserPermission.None && modulePermission == UserPermission.None)
        {
            permission = UserPermission.None;
            return false;
        }

        // Có area + module => cần role + permission
        if (areaMask != UserPermission.None && modulePermission != UserPermission.None)
        {
            permission = areaMask | modulePermission;
            return true;
        }

        // Không có area nhưng có modulePermission
        permission = modulePermission;
        return true;
    }

    /// <summary>
    /// CRUD theo HTTP method (REST):
    /// GET -> view, POST -> create, PUT/PATCH -> edit, DELETE -> delete
    /// </summary>
    public static bool TryResolveCrudByMethod(
        string method,
        UserPermission view,
        UserPermission create,
        UserPermission edit,
        UserPermission delete,
        out UserPermission permission)
    {
        permission = method switch
        {
            "GET" => view,
            "POST" => create,
            "PUT" or "PATCH" => edit,
            "DELETE" => delete,
            _ => UserPermission.None
        };

        return permission != UserPermission.None;
    }

    private static string? Normalize(string? routePattern)
    {
        if (string.IsNullOrWhiteSpace(routePattern)) return null;

        // rawText thường không có dấu '/' đầu, nhưng cứ normalize cho chắc
        return routePattern.Trim().Trim('/').ToLowerInvariant();
    }
}
