using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebBanHang.Core.Models;
namespace WebBanHang.Models;
public class OrderItem : Entity
{
    [Required]
    public Guid OrderId { get; set; }
    public Order Order { get; set; }= default!;

    [Required]
    public Guid ProductId { get; set; }
    public Product Product { get; set; }= default!;

    [Required]
    public int Quantity { get; set; }

    [Required]
    public decimal Price { get; set; }
}