## Overview

**Shoppy** là nền tảng thương mại điện tử (marketplace) kết nối **Admin – Seller – Người mua**, trong đó Seller đăng ký kinh doanh, quản lý sản phẩm/tồn kho/đơn hàng của mình, còn người mua duyệt và mua sản phẩm trên storefront. Hệ thống gồm Backend Web API (.NET 9, kiến trúc module `Modules/Identity`, `Catalog`, `Ordering`, `Seller`, `Notification`, `Payment`, `Cart`, PostgreSQL) và Frontend Angular SPA (`web_banhang_fe`) với 4 area: `publish`, `auth`, `admin`, `seller`.

Hệ thống bao gồm các nhóm chức năng chính:

- **Quản trị hệ thống (Admin Portal)**: dashboard thống kê toàn hệ thống (biểu đồ theo khoảng thời gian), quản lý user (danh sách phân trang, chi tiết, tạo user), quản lý seller (duyệt/từ chối đơn đăng ký bán hàng, xem chi tiết seller và hồ sơ đăng ký), quản lý danh mục sản phẩm dạng cây (thêm/sửa/xóa). *(Màn hình `ADM_*` tại `/admin/*`, guard `roleGuard(['Admin'])`; API `/api/v1/admin/*` — xem `Documents/screens/Screen_List.md` và `Documents/api/Api_list.md`.)*
- **Kênh bán hàng (Seller Portal)**: dashboard seller với biểu đồ doanh thu; quản lý sản phẩm (CRUD, upload ảnh lên Supabase Storage, đặt ảnh chính); quản lý tồn kho với cảnh báo sắp hết/hết hàng (`isLowStock`, `isOutOfStock`); xử lý đơn hàng (cập nhật trạng thái, hoàn tất — publish `OrderCompletedIntegrationEvent`, hủy — publish `OrderCancelledIntegrationEvent` để hoàn kho); thống kê doanh thu (tổng quan, biểu đồ, top sản phẩm bán chạy); cài đặt thông tin cửa hàng. *(Màn `SEL_DASHBOARD`, `SEL_PRODUCTS` tại `/seller/*`, guard `roleGuard(['Seller'])`; API `/api/v1/rseller/*`.)*
- **Đăng nhập / xác thực (Auth)**: đăng ký, đăng nhập email/password, OAuth (Google/Facebook) qua luồng `external-login-callback` + `finalize-login` với key tạm trong cache; làm mới token bằng refresh token. Token lưu HttpOnly cookie (`accessToken`, `refreshToken`); phân quyền theo role `Admin | Seller | User`. *(Màn `AUTH_SIGNIN`, `AUTH_SIGNUP`, `AUTH_ACCESS_DENIED`; API `/api/v1/auth/*`.)*
- **Thông báo toàn cục (Global Notification)**: danh sách thông báo của user (phân trang), số thông báo chưa đọc, đánh dấu đã đọc; dùng chung cho mọi role. *(API `/api/v1/global/notifications/*`.)*
- **Storefront công khai (Publish)**: trang chủ hiển thị sản phẩm cho khách truy cập chưa đăng nhập. *(Màn `PUB_HOME` tại `/`; area `publishRoutes`.)*
- **Nghiệp vụ đang phát triển**: giỏ hàng (`Cart`) và thanh toán (`Payment`) đã có domain/entity nhưng chưa phát sinh controller/API trong tài liệu hiện tại.

Tài liệu kèm theo: `Documents/api/Api_list.md` (51 API), `Documents/api/Api_Detail.md` (đặc tả chi tiết từng API), `Documents/screens/Screen_List.md` (16 màn hình).

Tổng thể, Shoppy đóng vai trò trung gian giúp Admin kiểm soát hệ thống và chất lượng seller, cung cấp cho Seller bộ công cụ vận hành bán hàng trọn vẹn (hàng hóa → tồn kho → đơn hàng → doanh thu), đồng thời hướng tới trải nghiệm mua sắm liền mạch cho người mua khi các module Cart/Payment hoàn thiện.
