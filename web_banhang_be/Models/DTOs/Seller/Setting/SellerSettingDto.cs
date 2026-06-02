namespace WebBanHang.Models.DTOs.Seller.Setting;
using System.ComponentModel.DataAnnotations;

public class SellerSettingDto
{

    [Required(ErrorMessage = "Tên cửa hàng không được để trống")]
    [MaxLength(100)]
    public string StoreName { get; set; } = null!;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Tên người chịu trách nhiệm/người gửi là bắt buộc")]
    public string RecipientName { get; set; } = null!;

    [Required(ErrorMessage = "Số điện thoại liên hệ là bắt buộc")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string PhoneNumber { get; set; } = null!;

    [MaxLength(200)]
    public string? AddressLine { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn Phường/Xã")]
    public string Ward { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng chọn Quận/Huyện")]
    public string District { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng chọn Tỉnh/Thành phố")]
    public string City { get; set; } = null!;

    [Required]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = null!;
}