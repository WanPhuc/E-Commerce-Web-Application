using System.ComponentModel.DataAnnotations;
using VanFucVN.Core.Persistence.Domain;

namespace AuraMart.Cart.Domain;

public class CartItem : Entity
{
    [Required]
    public Guid CartId { get; set; }
    public Cart Cart { get; set; } = default!;

    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [Range(0, 99999)]
    public int Quantity { get; set; }
}
