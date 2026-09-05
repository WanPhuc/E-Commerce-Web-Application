using System.ComponentModel.DataAnnotations;

using VanFucVN.Core.Persistence.Domain;

namespace AuraMart.Payment.Domain;

public class Payment : Entity
{
    [Required]
    public Guid OrderId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Method { get; set; } = default!;

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public string Status { get; set; } = "Pending";
}
