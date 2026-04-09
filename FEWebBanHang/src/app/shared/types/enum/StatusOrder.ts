export enum StatusOrder{
    Pending=0,      // Chờ xử lý
    Paid=1,         // Đã thanh toán
    Shipping=2,     // Đang trên đường giao đến khách hàng
    Completed=3,    // Hoàn thành
    Cancelled=4,     // Đã hủy
    Shipped=5,       // Đã giao cho đơn vị vận chuyển (Shipper đã lấy hàng)
    Processing=6,       // Shop đang đóng gói hàng

    // Nhóm trạng thái xử lý sự cố
    ReturnRequested=7,  // Khách yêu cầu trả hàng/hoàn tiền
    Returning=8,        // Hàng đang quay đầu về shop
    Returned=9,         // Shop đã nhận lại hàng thành công
    Refunded=10          // Đã hoàn tiền cho khách
}