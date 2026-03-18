using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using WebBanHang.Core.Models;

namespace WebBanHang.Models;
public class Address : Entity
{
    [Required(ErrorMessage = "Vui lòng nhập tên người nhận")]
    [MaxLength(100, ErrorMessage = "Tên người nhận không được vượt quá 100 ký tự")]
    [Display(Name = "Tên người nhận")]
    public string RecipientName { get; set; }= default!;


    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [MaxLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [Display(Name = "Số điện thoại")]
    public string PhoneNumber { get; set; }= default!;

    [MaxLength(200, ErrorMessage = "Số nhà,tên đường không được vượt quá 200 ký tự")]
    public string? AddressLine { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn Phường/Xã")]
    public string Ward { get; set; }= default!;

    [Required(ErrorMessage = "Vui lòng chọn Quận/Huyện")] 
    public string District { get; set; }= default!;

    [Required(ErrorMessage = "Vui lòng chọn Tỉnh/Thành phố")] 
    public string City { get; set; }= default!;

    public bool IsDefault { get; set; } = false;

    [Required]
    public Guid UserId { get; set; }
    [JsonIgnore]
    public User User { get; set; }= default!;
    [JsonIgnore]
    public Seller? Seller { get; set; }
}