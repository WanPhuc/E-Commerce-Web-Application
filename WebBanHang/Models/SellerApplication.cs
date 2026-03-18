using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebBanHang.Core.Models;
using WebBanHang.Models.Enums;
namespace WebBanHang.Models;

public class SellerApplication : Entity
{
    [Required]
    public Guid UserId { get; set; }
    public User User { get; set; }= default!;

    [Required(ErrorMessage = "Vui lòng nhập tên cửa hàng")]
    [MaxLength(150, ErrorMessage = "Tên cửa hàng không được vượt quá 150 ký tự")]
    public string ShopName { get; set; }= default!;

    [MaxLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
    public string? Description { get; set; }
    [Required(ErrorMessage = "Vui lòng nhập số điện thoại liên hệ kho")]
    public string PhoneNumber { get; set; } = default!;

    [Required(ErrorMessage = "Vui lòng chọn địa chỉ kho hàng")]
    public string City { get; set; } = default!;
    public string District { get; set; } = default!;
    public string Ward { get; set; } = default!;
    public string? AddressLine { get; set; }

    [Required]
    public SellerApplicationStatus Status { get; set; } = SellerApplicationStatus.Pending;// Pending, Approved, Rejected

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? ReviewedAt { get; set; }
}
