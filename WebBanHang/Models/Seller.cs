using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebBanHang.Core.Models;
using WebBanHang.Models.Enums;
namespace WebBanHang.Models;

public class Seller : Entity
{
    [Required(ErrorMessage = "Tên cửa hàng là bắt buộc.")]
    [MaxLength(100, ErrorMessage = "Tên cửa hàng không được vượt quá 100 ký tự.")]
    [Display(Name = "Tên cửa hàng")]
    public required string StoreName { get; set; }

    [MaxLength(500, ErrorMessage = "Mô tả cửa hàng không được vượt quá 500 ký tự.")]
    [Display(Name = "Mô tả cửa hàng")]
    public string? Description { get; set; }

    public SellerApplicationStatus Status { get; set; } = SellerApplicationStatus.Approved;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Required]
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    [Required(ErrorMessage = " Address is required.")]
    public Guid AddressId { get; set; }
    [ForeignKey("AddressId")]
    public Address Address { get; set; } = default!;

    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}