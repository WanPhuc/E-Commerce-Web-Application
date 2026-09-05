using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

using VanFucVN.Core.Persistence.Domain;

namespace AuraMart.Ordering.Domain;

public class Order : Entity
{
    [Required]
    public Guid UserId { get; set; }

    [MaxLength(50)]
    public string CustomerName { get; set; } = default!;

    [MaxLength(50)]
    public string CustomerEmail { get; set; } = default!;

    [Required]
    public Guid AddressId { get; set; }

    public OrderShippingAddress ShippingAddress { get; set; } = default!;

    [Required]
    [Precision(18, 2)]
    public decimal TotalAmount { get; set; }

    [Required]
    public string Status { get; set; } = "Pending";

    public DateTime? PaidAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

    public Guid SellerId { get; set; }
}
