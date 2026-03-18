namespace WebBanHang.Models.Enums;

public enum PaymentStatus
{
    Pending,      // Chờ thanh toán
    Paid,         // Đã thanh toán
    Failed,       // Thất bại
    Refund,       // Hoàn tiền
    Cancelled     // Hủy thanh toán
}
