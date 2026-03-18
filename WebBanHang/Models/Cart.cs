using System.ComponentModel.DataAnnotations;
using WebBanHang.Core.Models;
namespace WebBanHang.Models;
public class Cart : Entity
{
    [Required]
    public required Guid UserId { get; set; }

    public User User { get; set; }= default!;

    public ICollection<CartItem> CartItems { get; set; }= new List<CartItem>();
}