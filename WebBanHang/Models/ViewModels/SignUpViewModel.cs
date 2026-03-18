using System.ComponentModel.DataAnnotations;
namespace WebBanHang.Models.ViewModels;
public class SignupViewModel()
{
    [Required(ErrorMessage = "Họ và tên là bắt buộc.")]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email là bắt buộc.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string Password { get; set; } = string.Empty;

    

    public string? ReturnUrl { get; set; }
}
