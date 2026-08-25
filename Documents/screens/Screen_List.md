# Screen List

Danh sách màn hình hiện tại của FE (`web_banhang_fe`, Angular SPA). Nguồn: `src/app/app.routes.ts` và các file `*.routes.ts` trong `src/app/areas/`.

| No | Screen ID | Name | Route/Path | Platform |
|---|---|---|---|---|
| 1 | PUB_HOME | Trang chủ | `/` | Web |
| 2 | AUTH_SIGNIN | Đăng nhập | `/auth/signin` | Web |
| 3 | AUTH_SIGNUP | Đăng ký | `/auth/signup` | Web |
| 4 | AUTH_ACCESS_DENIED | Từ chối truy cập | `/auth/accessdenied` | Web |
| 5 | ADM_DASHBOARD | Admin Dashboard | `/admin` | Web (role Admin) |
| 6 | ADM_SELLER_LIST | Quản lý seller | `/admin/seller` | Web (role Admin) |
| 7 | ADM_SELLER_DETAIL | Chi tiết seller | `/admin/seller/seller/:id` | Web (role Admin) |
| 8 | ADM_SELLER_APPLICATION_DETAIL | Chi tiết đơn đăng ký bán hàng | `/admin/seller/application-seller/:id` | Web (role Admin) |
| 9 | ADM_USER_LIST | Quản lý user | `/admin/user` | Web (role Admin) |
| 10 | ADM_USER_DETAIL | Chi tiết user | `/admin/user/detail/:id` | Web (role Admin) |
| 11 | ADM_CATEGORY_LIST | Quản lý danh mục | `/admin/category` | Web (role Admin) |
| 12 | ADM_CATEGORY_DETAIL | Chi tiết danh mục | `/admin/category/detail/:id` | Web (role Admin) |
| 13 | ADM_CATEGORY_CREATE | Tạo danh mục | `/admin/category/create` | Web (role Admin) |
| 14 | ADM_CATEGORY_UPDATE | Cập nhật danh mục | `/admin/category/update/:id` | Web (role Admin) |
| 15 | SEL_DASHBOARD | Seller Dashboard | `/seller` | Web (role Seller) |
| 16 | SEL_PRODUCTS | Sản phẩm của seller | `/seller/products` | Web (role Seller) |

## Ghi chú cấu trúc route

- `/` → `publishRoutes` (`areas/publish`) — công khai.
- `/auth` → `authRoutes` (`areas/auth`, layout `AuthLayoutComponent`).
- `/admin` → `adminRoutes` (`areas/admin`, layout `AdminLayoutComponent`, guard `roleGuard(['Admin'])`).
- `/seller` → `sellerRoutes` (`areas/seller`, layout `SellerLayoutComponent`, guard `roleGuard(['Seller'])`).

