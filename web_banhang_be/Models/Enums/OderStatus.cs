namespace WebBanHang.Models.Enums;
public enum OrderStatus
{
    Pending,      // Chờ xử lý
    Paid,         // Đã thanh toán
    Shipping,     // Đang trên đường giao đến khách hàng
    Completed,    // Hoàn thành
    Cancelled,     // Đã hủy
    Shipped,       // Đã giao cho đơn vị vận chuyển (Shipper đã lấy hàng)
    Processing,       // Shop đang đóng gói hàng

    // Nhóm trạng thái xử lý sự cố
    ReturnRequested,  // Khách yêu cầu trả hàng/hoàn tiền
    Returning,        // Hàng đang quay đầu về shop
    Returned,         // Shop đã nhận lại hàng thành công
    Refunded          // Đã hoàn tiền cho khách
}