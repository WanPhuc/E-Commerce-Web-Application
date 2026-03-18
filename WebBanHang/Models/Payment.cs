using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebBanHang.Core.Models;
using WebBanHang.Models.Enums;

namespace WebBanHang.Models;

public class Payment : Entity
{
    [Required]
    public Guid OrderId { get; set; }
    public Order Order { get; set; }= default!;

    [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
    [MaxLength(50)]
    public string Method { get; set; }= default!; // COD, Momo...

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public PaymentStatus Status { get; set; }  = PaymentStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
