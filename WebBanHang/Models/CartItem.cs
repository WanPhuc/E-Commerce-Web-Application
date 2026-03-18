using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebBanHang.Core.Models;
namespace WebBanHang.Models;
public class CartItem : Entity
{
    [Required]
    public Guid CartId { get; set; }
    public Cart Cart { get; set; }= default!;

    [Required]
    public Guid ProductId { get; set; }
    public Product Product { get; set; }= default!;

    [Required]
    [Range(0,99999)]
    public int Quantity { get; set; } 
}