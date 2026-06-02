public enum ProductStatus
{
    // --- Nhóm khởi tạo ---
    Draft,        // Seller lưu nháp, chưa muốn đăng (chưa hiện lên sàn)
    Pending,      // Đang chờ Admin duyệt (sau khi Seller nhấn "Đăng")
    
    // --- Nhóm hoạt động ---
    Active,       // Đang bán bình thường
    OutOfStock,   // Hết hàng (Hệ thống tự động chuyển sang khi Stock = 0)
    
    // --- Nhóm tạm dừng/Khóa ---
    Hidden,       // Seller chủ động tạm ẩn sản phẩm (không muốn bán lúc này)
    Blocked,      // Bị Admin khóa (do vi phạm chính sách, hàng giả...)
    
    // --- Nhóm kết thúc ---
    Discontinued, // Ngừng kinh doanh (Vẫn giữ dữ liệu để xem lại đơn hàng cũ nhưng không bao giờ bán lại)
    Deleted       // Xóa mềm (Dùng thay cho việc xóa cứng khỏi Database)
}