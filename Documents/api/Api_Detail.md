# API Detail

Đặc tả chi tiết API của dự án Shoppy. Danh sách ID tham chiếu `Api_list.md`.

Mọi response chuẩn qua envelope `ApiResponse<T>`:

```json
{
  "status": 200,
  "message": "string",
  "code": "string",
  "data": "object | array | null (tùy API)"
}
```

Xác thực bằng HttpOnly cookie: `accessToken`, `refreshToken` (FE gửi kèm `withCredentials`). Phân quyền theo role qua filter `[AuthorizeRole]`.

## Auth

### API Sign In
Đăng nhập bằng email/password, trả về user hiện tại + tokens và set cookie.
API ID: AUTH_SIGNIN
Method: POST
URL: /api/v1/auth/signin
Request Headers:
- (optional) None
Query Params:
- None
Request Body:
```json
{
  "email": "user@example.com",
  "password": "string"
}
```
Response Body:
```json
{
  "status": 200,
  "message": "Login success",
  "code": "string",
  "data": {
    "me": { "id": "guid", "fullName": "string", "email": "user@example.com", "role": "Admin | Seller | User" },
    "tokens": { "accessToken": "string", "refreshToken": "string", "accessTokenExpiresAt": "2026-01-01T00:00:00Z" }
  }
}
```
Notes:
- Token được ghi vào cookie HttpOnly; lỗi validate email/password trả 400.

### API Sign Up
Đăng ký tài khoản mới, sau khi tạo thành công tự đăng nhập (set cookie).
API ID: AUTH_SIGNUP
Method: POST
URL: /api/v1/auth/signup
Request Body:
```json
{
  "fullName": "string",
  "email": "user@example.com",
  "password": "string",
  "returnUrl": "string | null"
}
```
Response Body: giống API Sign In (`AuthResponseDto`).

### API Refresh Token
Làm mới access token từ cookie `refreshToken`.
API ID: AUTH_REFRESH_TOKEN
Method: POST
URL: /api/v1/auth/refresh-token
Request Headers:
- Cookie: accessToken, refreshToken
Request Body:
- None
Response Body:
```json
{
  "status": 200,
  "message": "Token refreshed",
  "code": "string",
  "data": { "accessToken": "string", "refreshToken": "string", "accessTokenExpiresAt": "2026-01-01T00:00:00Z" }
}
```

### API Logout
Thu hồi refresh token và xoá cookie đăng nhập.
API ID: AUTH_LOGOUT
Method: POST
URL: /api/v1/auth/logout
Request Headers:
- Cookie: refreshToken
Response Body: envelope không có `data`. Không tìm thấy token trả 400 (`AUTH_TOKEN_NOT_FOUND`).

### API Get Me
Thông tin user đang đăng nhập.
API ID: AUTH_ME
Method: GET
URL: /api/v1/auth/me
Request Headers:
- Cookie: accessToken
Response Body:
```json
{
  "status": 200,
  "message": "Success",
  "code": "string",
  "data": { "id": "guid", "fullName": "string", "email": "user@example.com", "role": "Admin | Seller | User" }
}
```

### API External Login
Bắt đầu OAuth login (Google/Facebook...), redirect tới provider.
API ID: AUTH_EXTERNAL_LOGIN
Method: GET
URL: /api/v1/auth/external-login?provider={provider}
Query Params:
- provider: (string) tên provider, bắt buộc
Response: redirect 302 tới trang xác thực của provider.

### API External Login Callback
Nhận callback từ provider, cache kết quả login rồi redirect về FE kèm key.
API ID: AUTH_EXTERNAL_LOGIN_CALLBACK
Method: GET
URL: /api/v1/auth/external-login-callback?provider={provider}
Response: redirect tới `{BaseUrl}/auth-callback?key={key}`.

### API Finalize Login
FE dùng `key` nhận từ callback để hoàn tất phiên đăng nhập.
API ID: AUTH_FINALIZE_LOGIN
Method: GET
URL: /api/v1/auth/finalize-login?key={key}
Query Params:
- key: (string) key trong cache (hết hạn sau 60 giây)
Response Body: envelope với `data` = `MeDto`; đồng thời set cookie token.

## Global Notification

### API Get My Notifications
Danh sách thông báo của user hiện tại.
API ID: NOTI_GET_MY_NOTIFICATIONS
Method: GET
URL: /api/v1/global/notifications
Request Headers:
- Cookie: accessToken
Query Params:
- page: (int) mặc định 1
- pageSize: (int) mặc định 10, tối đa 100
Response Body:
```json
{
  "status": 200,
  "message": "Success",
  "code": "string",
  "data": [
    { "id": "guid", "title": "string", "message": "string", "redirectUrl": "string | null", "type": "NotificationType", "isRead": false, "createdAt": "2026-01-01T00:00:00Z" }
  ]
}
```

### API Get Unread Count
So thong bao chua doc cua user hien tai.
API ID: NOTI_GET_UNREAD_COUNT
Method: GET
URL: /api/v1/global/notifications/unread-count
Response Body: envelope voi `data` = so nguyen.

### API Mark As Read
Danh dau mot thong bao da doc.
API ID: NOTI_MARK_AS_READ
Method: PATCH
URL: /api/v1/global/notifications/{notiId}/read
Path Params:
- notiId: (guid) id thong bao
Response Body: envelope khong co `data`.

## Admin

Yeu cau role **Admin** cho toan bo API trong muc nay.

### API Admin Dashboard Stats
So lieu tong hop he thong (user, seller, product, order...).
API ID: ADMIN_DASHBOARD_STATS
Method: GET
URL: /api/v1/admin
Response Body: envelope voi `data` = doi tuong thong ke dashboard.

### API Admin Dashboard Chart
Du lieu bieu do theo khoang thoi gian.
API ID: ADMIN_DASHBOARD_CHART
Method: GET
URL: /api/v1/admin/chart?range={range}
Query Params:
- range: (DashboardRanger) khoang thoi gian ve bieu do
Response Body: envelope voi `data` = chuoi diem du lieu bieu do.

### API Admin User List
Danh sach user, phan trang.
API ID: ADMIN_USER_LIST
Method: GET
URL: /api/v1/admin/users?page=&pageSize=
Query Params:
- page: (int) mac dinh 1
- pageSize: (int) mac dinh 10, toi da 100
Response Body: envelope voi `data` = danh sach user phan trang.

### API Admin User Detail
Chi tiet mot user.
API ID: ADMIN_USER_DETAIL
Method: GET
URL: /api/v1/admin/users/detail/{id}
Path Params:
- id: (guid) id user

### API Admin Create User
Tao user moi boi admin.
API ID: ADMIN_USER_CREATE
Method: POST
URL: /api/v1/admin/users/create
Request Body:
```json
{
  "fullName": "string",
  "email": "user@example.com",
  "password": "string"
}
```

### API Admin Category List
Cay danh muc san pham, ho tro phan trang.
API ID: ADMIN_CATEGORY_LIST
Method: GET
URL: /api/v1/admin/categories?page=&pageSize=
Query Params:
- page, pageSize: nhu tren
Response Body:
```json
{
  "status": 200,
  "message": "Success",
  "code": "string",
  "data": [
    { "id": "guid", "name": "string", "parentId": "guid | null", "children": [ /* CategoryDto */ ] }
  ]
}
```

### API Admin Category Detail
Chi tiet mot danh muc.
API ID: ADMIN_CATEGORY_DETAIL
Method: GET
URL: /api/v1/admin/categories/detail/{id}

### API Admin Create Category
Tao danh muc moi.
API ID: ADMIN_CATEGORY_CREATE
Method: POST
URL: /api/v1/admin/categories/create
Request Body:
```json
{ "name": "string", "parentId": "guid | null" }
```

### API Admin Update Category
Cap nhat danh muc.
API ID: ADMIN_CATEGORY_UPDATE
Method: PUT
URL: /api/v1/admin/categories/update/{id}
Request Body:
```json
{ "name": "string", "parentId": "guid | null" }
```

### API Admin Delete Category
Xoa danh muc theo id.
API ID: ADMIN_CATEGORY_DELETE
Method: DELETE
URL: /api/v1/admin/categories/delete/{id}

### API Admin Seller List
Danh sach seller va don dang ky ban hang, phan trang.
API ID: ADMIN_SELLER_LIST
Method: GET
URL: /api/v1/admin/sellers?page=&pageSize=
Query Params:
- page, pageSize: nhu tren

### API Admin Approve Seller Application
Duyet don dang ky ban hang.
API ID: ADMIN_SELLER_APPLICATION_APPROVE
Method: POST
URL: /api/v1/admin/sellers/{applicationId}/approved
Path Params:
- applicationId: (guid) id don dang ky

### API Admin Reject Seller Application
Tu choi don dang ky ban hang.
API ID: ADMIN_SELLER_APPLICATION_REJECT
Method: POST
URL: /api/v1/admin/sellers/{applicationId}/rejected
Path Params:
- applicationId: (guid) id don dang ky

### API Admin Seller Detail
Chi tiet seller da duyet.
API ID: ADMIN_SELLER_DETAIL
Method: GET
URL: /api/v1/admin/sellers/sellers/{sellerId}
Path Params:
- sellerId: (guid) id seller

### API Admin Seller Application Detail
Chi tiet don dang ky ban hang.
API ID: ADMIN_SELLER_APPLICATION_DETAIL
Method: GET
URL: /api/v1/admin/sellers/application-seller/{applicationId}
Path Params:
- applicationId: (guid) id don dang ky

## Seller

Yeu cau role **Seller** cho toan bo API trong muc nay. Route goc: `/api/v1/rseller/*`.

### API Seller Product List
San pham cua seller, phan trang.
API ID: SELLER_PRODUCT_LIST
Method: GET
URL: /api/v1/rseller/products?page=&pageSize=
Query Params:
- page: (int) mac dinh 1
- pageSize: (int) mac dinh 10, toi da 100

### API Seller Product Detail
Chi tiet san pham cua seller.
API ID: SELLER_PRODUCT_DETAIL
Method: GET
URL: /api/v1/rseller/products/detail/{productId}
Path Params:
- productId: (guid)

### API Seller Create Product
Tao san pham moi.
API ID: SELLER_PRODUCT_CREATE
Method: POST
URL: /api/v1/rseller/products/create
Request Body:
```json
{
  "name": "string",
  "sku": "string",
  "price": 0,
  "stock": 0,
  "lowStockThreshold": 0,
  "discountPercent": 0,
  "categoryId": "guid",
  "images": [ { "imageUrl": "string", "isMainImage": false } ]
}
```
Notes:
- `status` mac dinh `Active`; truong theo `CreateSellerProductDto`.

### API Seller Update Product
Cap nhat san pham.
API ID: SELLER_PRODUCT_UPDATE
Method: PUT
URL: /api/v1/rseller/products/update/{productId}
Request Body:
```json
{
  "name": "string",
  "sku": "string",
  "price": 0,
  "stock": 0,
  "lowStockThreshold": 0,
  "status": "Active",
  "discountPercent": 0,
  "categoryId": "guid"
}
```

### API Seller Delete Product
Xoa san pham.
API ID: SELLER_PRODUCT_DELETE
Method: DELETE
URL: /api/v1/rseller/products/delete/{productId}

### API Seller Change Product Status
Doi trang thai san pham.
API ID: SELLER_PRODUCT_CHANGE_STATUS
Method: PUT
URL: /api/v1/rseller/products/change-status/{productId}?newStatus={ProductStatus}
Query Params:
- newStatus: (ProductStatus) gia tri enum trang thai san pham

### API Seller Upload Product Image
Upload anh len Supabase Storage, tra ve URL.
API ID: SELLER_PRODUCT_IMAGE_UPLOAD
Method: POST
URL: /api/v1/rseller/products/images/upload
Request Body: `multipart/form-data`, field `file`.
Response Body: envelope voi `data` = URL string. Sai dinh dang/kich thuoc tra 400.

### API Seller Add Product Image
Them record anh cho san pham.
API ID: SELLER_PRODUCT_IMAGE_ADD
Method: POST
URL: /api/v1/rseller/products/{productId}/images
Request Body:
```json
{ "imageUrl": "string", "isMainImage": false }
```

### API Seller Update Product Image
Cap nhat record anh.
API ID: SELLER_PRODUCT_IMAGE_UPDATE
Method: PUT
URL: /api/v1/rseller/products/{productId}/images/{imageId}
Request Body: nhu Add Product Image.

### API Seller Delete Product Image
Xoa record anh.
API ID: SELLER_PRODUCT_IMAGE_DELETE
Method: DELETE
URL: /api/v1/rseller/products/{productId}/images/{imageId}

### API Seller Set Main Product Image
Dat anh chinh cua san pham.
API ID: SELLER_PRODUCT_IMAGE_SET_MAIN
Method: PUT
URL: /api/v1/rseller/products/{productId}/images/{imageId}/set-main

### API Seller Inventory List
Ton kho san pham cua seller, phan trang + filter.
API ID: SELLER_INVENTORY_LIST
Method: GET
URL: /api/v1/rseller/inventory?page=&pageSize=&filter=
Query Params:
- page, pageSize: nhu tren
- filter: (string, optional) loc trang thai ton kho
Response Body:
```json
{
  "status": 200,
  "message": "Success",
  "code": "string",
  "data": [
    {
      "id": "guid", "name": "string", "sku": "string",
      "stock": 0, "lowStockThreshold": 0, "status": "Active",
      "imageUrl": "string | null", "price": 0,
      "isLowStock": false, "isOutOfStock": false
    }
  ]
}
```

### API Seller Update Inventory
Cap nhat ton kho san pham.
API ID: SELLER_INVENTORY_UPDATE
Method: PATCH
URL: /api/v1/rseller/inventory/{productId}
Request Body:
```json
{ "stock": 0, "lowStockThreshold": 0, "status": "Active" }
```
Notes:
- Cac truong optional, chi gui truong can cap nhat (`UpdateSellerInventoryDto`).

### API Seller Order List
Don hang cua seller, phan trang.
API ID: SELLER_ORDER_LIST
Method: GET
URL: /api/v1/rseller/orders?page=&pageSize=

### API Seller Order Detail
Chi tiet don hang.
API ID: SELLER_ORDER_DETAIL
Method: GET
URL: /api/v1/rseller/orders/{orderId}

### API Seller Update Order Status
Cap nhat trang thai don hang.
API ID: SELLER_ORDER_UPDATE_STATUS
Method: PATCH
URL: /api/v1/rseller/orders/{orderId}/status?newStatus={OrderStatus}
Query Params:
- newStatus: (OrderStatus) gia tri enum trang thai don hang

### API Seller Complete Order
Hoan tat don da thanh toan, publish `OrderCompletedIntegrationEvent`.
API ID: SELLER_ORDER_COMPLETE
Method: POST
URL: /api/v1/rseller/orders/{orderId}/complete

### API Seller Cancel Order
Huy don, publish `OrderCancelledIntegrationEvent` (consumer hoan stock).
API ID: SELLER_ORDER_CANCEL
Method: POST
URL: /api/v1/rseller/orders/{orderId}/cancel
Request Body:
```json
"ly do huy"
```

### API Seller Dashboard Stats
So lieu tong hop cua seller.
API ID: SELLER_DASHBOARD_STATS
Method: GET
URL: /api/v1/rseller/dashboard

### API Seller Dashboard Chart
Bieu do dashboard theo khoang thoi gian.
API ID: SELLER_DASHBOARD_CHART
Method: GET
URL: /api/v1/rseller/dashboard/chartdashboard?ranger={ranger}
Query Params:
- ranger: (ChartRanger) khoang thoi gian

### API Seller Revenue Summary
Tong quan doanh thu cua seller.
API ID: SELLER_REVENUE_SUMMARY
Method: GET
URL: /api/v1/rseller/revenue

### API Seller Revenue Chart
Bieu do doanh thu theo khoang thoi gian.
API ID: SELLER_REVENUE_CHART
Method: GET
URL: /api/v1/rseller/revenue/chart?range={range}
Query Params:
- range: (ChartRanger)

### API Seller Top Selling Products
Top san pham ban chay, phan trang.
API ID: SELLER_REVENUE_TOP_SELLING
Method: GET
URL: /api/v1/rseller/revenue/top-selling?page=&pageSize=

### API Seller Get Settings
Cau hinh shop cua seller.
API ID: SELLER_SETTING_GET
Method: GET
URL: /api/v1/rseller/settings
Response Body: envelope voi `data` = `SellerSettingDto`.

### API Seller Update Settings
Cap nhat cau hinh shop.
API ID: SELLER_SETTING_UPDATE
Method: PATCH
URL: /api/v1/rseller/settings/update
Request Body:
```json
{
  "storeName": "string (bat buoc, max 100)",
  "description": "string | null (max 500)",
  "recipientName": "string (bat buoc)",
  "phoneNumber": "string (bat buoc)",
  "addressLine": "string (max 200)"
}
```
Notes:
- Truong day du tham khao `Modules/Seller/Dtos/SellerSettingDto.cs`.

