export enum ProductStatus{
    // --- Nhóm khởi tạo ---
    Draft=0,        // Seller lưu nháp, chưa muốn đăng (chưa hiện lên sàn)
    Pending=1,      // Đang chờ Admin duyệt (sau khi Seller nhấn "Đăng")

    // --- Nhóm hoạt động ---
    Active=2,       // Đang bán bình thường
    OutOfStock=3,   // Hết hàng (Hệ thống tự động chuyển sang khi Stock = 0)

    // --- Nhóm tạm dừng/Khóa ---
    Hidden=4,       // Seller chủ động tạm ẩn sản phẩm (không muốn bán lúc này)
    Blocked=5,      // Bị Admin khóa (do vi phạm chính sách, hàng giả...)

    // --- Nhóm kết thúc ---
    Discontinued=6, // Ngừng kinh doanh (Vẫn giữ dữ liệu để xem lại đơn hàng cũ nhưng không bao giờ bán lại)
    Deleted=7       // Xóa mềm (Dùng thay cho việc xóa cứng khỏi Database)
}