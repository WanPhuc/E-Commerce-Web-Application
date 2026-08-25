# API List

Danh sách API hiện có trong dự án Shoppy (quét từ source `web_banhang_be`). Base URL dev: `http://localhost:7100/api/v1`.

## Auth (`Modules/Identity/Controllers/AuthController.cs`) — Route gốc: `/api/v1/auth`

| No | API ID | Method | Route | Permission |
|---|---|---|---|---|
| 1 | AUTH_SIGNIN | POST | `/api/v1/auth/signin` | Public |
| 2 | AUTH_SIGNUP | POST | `/api/v1/auth/signup` | Public |
| 3 | AUTH_REFRESH_TOKEN | POST | `/api/v1/auth/refresh-token` | Public |
| 4 | AUTH_LOGOUT | POST | `/api/v1/auth/logout` | Authenticated |
| 5 | AUTH_ME | GET | `/api/v1/auth/me` | Authenticated |
| 6 | AUTH_EXTERNAL_LOGIN | GET | `/api/v1/auth/external-login?provider=` | Public |
| 7 | AUTH_EXTERNAL_LOGIN_CALLBACK | GET | `/api/v1/auth/external-login-callback?provider=` | Public |
| 8 | AUTH_FINALIZE_LOGIN | GET | `/api/v1/auth/finalize-login?key=` | Public |

## Global Notification (`Modules/Notification/Controllers/NotificationController.cs`) — Route gốc: `/api/v1/global/notifications`

| No | API ID | Method | Route | Permission |
|---|---|---|---|---|
| 9 | NOTI_GET_MY_NOTIFICATIONS | GET | `/api/v1/global/notifications` | Authenticated |
| 10 | NOTI_GET_UNREAD_COUNT | GET | `/api/v1/global/notifications/unread-count` | Authenticated |
| 11 | NOTI_MARK_AS_READ | PATCH | `/api/v1/global/notifications/{notiId}/read` | Authenticated |

## Admin — `api/v1/admin` (Dashboard · Users · Categories · Sellers)

_Nguồn: `Controllers/Composition/DashboardController.cs`, `Modules/Identity/Controllers/UserController.cs`, `Modules/Catalog/Controllers/CategoryController.cs`, `Modules/Seller/Controllers/SellerController.cs`_

| No | API ID | Method | Route | Permission |
|---|---|---|---|---|
| 12 | ADMIN_DASHBOARD_STATS | GET | `/api/v1/admin` | Admin |
| 13 | ADMIN_DASHBOARD_CHART | GET | `/api/v1/admin/chart` | Admin |
| 14 | ADMIN_USER_LIST | GET | `/api/v1/admin/users` | Admin |
| 15 | ADMIN_USER_DETAIL | GET | `/api/v1/admin/users/detail/{id}` | Admin |
| 16 | ADMIN_USER_CREATE | POST | `/api/v1/admin/users/create` | Admin |
| 17 | ADMIN_CATEGORY_LIST | GET | `/api/v1/admin/categories` | Admin |
| 18 | ADMIN_CATEGORY_DETAIL | GET | `/api/v1/admin/categories/detail/{id}` | Admin |
| 19 | ADMIN_CATEGORY_CREATE | POST | `/api/v1/admin/categories/create` | Admin |
| 20 | ADMIN_CATEGORY_UPDATE | PUT | `/api/v1/admin/categories/update/{id}` | Admin |
| 21 | ADMIN_CATEGORY_DELETE | DELETE | `/api/v1/admin/categories/delete/{id}` | Admin |
| 22 | ADMIN_SELLER_LIST | GET | `/api/v1/admin/sellers` | Admin |
| 23 | ADMIN_SELLER_APPLICATION_APPROVE | POST | `/api/v1/admin/sellers/{applicationId}/approved` | Admin |
| 24 | ADMIN_SELLER_APPLICATION_REJECT | POST | `/api/v1/admin/sellers/{applicationId}/rejected` | Admin |
| 25 | ADMIN_SELLER_DETAIL | GET | `/api/v1/admin/sellers/sellers/{sellerId}` | Admin |
| 26 | ADMIN_SELLER_APPLICATION_DETAIL | GET | `/api/v1/admin/sellers/application-seller/{applicationId}` | Admin |

## Seller — `api/v1/rseller/*` (Products & Images · Inventory · Orders · Dashboard · Revenue · Settings)

_Nguồn: `ProductSellerController.cs`, `SellerInventoryController.cs`, `SellerOrderController.cs`, `SellerDashboard.Controller.cs`, `SellerRevenueController.cs`, `SellerSettingController.cs`_

| No | API ID | Method | Route | Permission |
|---|---|---|---|---|
| 27 | SELLER_PRODUCT_LIST | GET | `/api/v1/rseller/products` | Seller |
| 28 | SELLER_PRODUCT_DETAIL | GET | `/api/v1/rseller/products/detail/{productId}` | Seller |
| 29 | SELLER_PRODUCT_CREATE | POST | `/api/v1/rseller/products/create` | Seller |
| 30 | SELLER_PRODUCT_UPDATE | PUT | `/api/v1/rseller/products/update/{productId}` | Seller |
| 31 | SELLER_PRODUCT_DELETE | DELETE | `/api/v1/rseller/products/delete/{productId}` | Seller |
| 32 | SELLER_PRODUCT_CHANGE_STATUS | PUT | `/api/v1/rseller/products/change-status/{productId}` | Seller |
| 33 | SELLER_PRODUCT_IMAGE_UPLOAD | POST | `/api/v1/rseller/products/images/upload` | Seller |
| 34 | SELLER_PRODUCT_IMAGE_ADD | POST | `/api/v1/rseller/products/{productId}/images` | Seller |
| 35 | SELLER_PRODUCT_IMAGE_UPDATE | PUT | `/api/v1/rseller/products/{productId}/images/{imageId}` | Seller |
| 36 | SELLER_PRODUCT_IMAGE_DELETE | DELETE | `/api/v1/rseller/products/{productId}/images/{imageId}` | Seller |
| 37 | SELLER_PRODUCT_IMAGE_SET_MAIN | PUT | `/api/v1/rseller/products/{productId}/images/{imageId}/set-main` | Seller |
| 38 | SELLER_INVENTORY_LIST | GET | `/api/v1/rseller/inventory` | Seller |
| 39 | SELLER_INVENTORY_UPDATE | PATCH | `/api/v1/rseller/inventory/{productId}` | Seller |
| 40 | SELLER_ORDER_LIST | GET | `/api/v1/rseller/orders` | Seller |
| 41 | SELLER_ORDER_DETAIL | GET | `/api/v1/rseller/orders/{orderId}` | Seller |
| 42 | SELLER_ORDER_UPDATE_STATUS | PATCH | `/api/v1/rseller/orders/{orderId}/status` | Seller |
| 43 | SELLER_ORDER_COMPLETE | POST | `/api/v1/rseller/orders/{orderId}/complete` | Seller |
| 44 | SELLER_ORDER_CANCEL | POST | `/api/v1/rseller/orders/{orderId}/cancel` | Seller |
| 45 | SELLER_DASHBOARD_STATS | GET | `/api/v1/rseller/dashboard` | Seller |
| 46 | SELLER_DASHBOARD_CHART | GET | `/api/v1/rseller/dashboard/chartdashboard` | Seller |
| 47 | SELLER_REVENUE_SUMMARY | GET | `/api/v1/rseller/revenue` | Seller |
| 48 | SELLER_REVENUE_CHART | GET | `/api/v1/rseller/revenue/chart` | Seller |
| 49 | SELLER_REVENUE_TOP_SELLING | GET | `/api/v1/rseller/revenue/top-selling` | Seller |
| 50 | SELLER_SETTING_GET | GET | `/api/v1/rseller/settings` | Seller |
| 51 | SELLER_SETTING_UPDATE | PATCH | `/api/v1/rseller/settings/update` | Seller |

