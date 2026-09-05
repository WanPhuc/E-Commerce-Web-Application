namespace AuraMart.Payment.Contracts;

public class CreatePaymentRequest
{
    public Guid OrderId { get; set; }
    public string Method { get; set; } = "COD"; // COD, VNPay, MoMo
    public decimal Amount { get; set; }
}

public class PaymentResponseDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string Method { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
