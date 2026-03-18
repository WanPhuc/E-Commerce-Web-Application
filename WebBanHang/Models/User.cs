using System.ComponentModel.DataAnnotations;
using WebBanHang.Core.Models;

namespace WebBanHang.Models;
public class User : Entity
{
    [Required(ErrorMessage = "Họ và tên là bắt buộc.")]
    [MaxLength(50, ErrorMessage = "Họ và tên không được vượt quá 50 ký tự.")]
    [Display(Name = "Họ và tên")]
    public required string FullName { get; set; }

    [Required(ErrorMessage = "Email là bắt buộc.")]
    [MaxLength(50, ErrorMessage = "Email không được vượt quá 50 ký tự.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [Display(Name = "Email")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
    [MaxLength(50, ErrorMessage = "Mật khẩu không được vượt quá 50 ký tự.")]
    [Display(Name = "Mật khẩu")]
    public required string PasswordHash { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public bool IsActive { get; set; } = true;

    [Required]
    public required Guid RoleId { get; set; }
    public Role Role { get; set; }= default!;

    public Seller? Seller { get; set; }
    public ICollection<Cart> Carts { get; set; }= new List<Cart>();
    public ICollection<Address> Addresses { get; set; }= new List<Address>();
    public ICollection<Order> Orders { get; set; }= new List<Order>();
    public ICollection<SellerApplication> SellerApplications { get; set; }= new List<SellerApplication>();
}