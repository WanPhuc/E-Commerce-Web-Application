using System.ComponentModel.DataAnnotations;
using VanFucVN.Core.Persistence.Domain;

namespace AuraMart.Cart.Domain;

public class Cart : Entity
{
    [Required]
    public Guid UserId { get; set; }

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
