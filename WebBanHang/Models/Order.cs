using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Core.Models;
using WebBanHang.Models.Enums;
namespace WebBanHang.Models;
public class Order : Entity
{
    [Required]
    public Guid UserId { get; set; }
    public User User { get; set; }= default!;

    [Required]
    public Guid AddressId { get; set; }
    public Address Address { get; set; }= default!;

    [Required]
    [Precision(18,2)]
    public decimal TotalAmount { get; set; }

    [Required]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? PaidAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public ICollection<OrderItem> Items { get; set; }=new List<OrderItem>();
    public Payment? Payment { get; set; } 
    public Guid SellerId { get; set; }
    public Seller Seller { get; set; }= default!;

}