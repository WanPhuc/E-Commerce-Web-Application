using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BuildingBlocks.EventBus;
using BuildingBlocks.Outbox;
using AuraMart.Payment.Infrastructure.Persistence;
using AuraMart.Payment.Domain;
using System.Text.Json;

namespace AuraMart.Payment.Api.Controllers;

[ApiController]
[Route("internal/payments")]
public class InternalPaymentController : ControllerBase
{
    public static string InternalApiKey { get; set; } = "dev-internal-key";
    private readonly PaymentDbContext _db;

    public InternalPaymentController(PaymentDbContext db)
    {
        _db = db;
    }

    private bool IsAuthorized() =>
        Request.Headers.TryGetValue("X-Internal-Api-Key", out var key) && key == InternalApiKey;

    private IActionResult Denied() => StatusCode(403, new { error = "forbidden" });

    // POST internal/payments/process
    [HttpPost("process")]
    public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentRequest req)
    {
        if (!IsAuthorized()) return Denied();

        var payment = new AuraMart.Payment.Domain.Payment
        {
            Id = Guid.NewGuid(),
            OrderId = req.OrderId,
            Method = req.Method,
            Amount = req.Amount,
            Status = "Completed"
        };
        _db.Payments.Add(payment);

        // Publish PaymentSucceeded event via outbox
        var evt = new PaymentSucceededIntegrationEvent
        {
            OrderId = req.OrderId,
            PaymentId = payment.Id,
            Method = req.Method,
            Amount = req.Amount
        };

        _db.OutboxMessages.Add(new OutboxMessage
        {
            Id = evt.EventId,
            Type = evt.GetType().AssemblyQualifiedName ?? evt.GetType().Name,
            Payload = JsonSerializer.Serialize(evt),
            OccurredOnUtc = evt.OccurredOnUtc
        });

        await _db.SaveChangesAsync();

        return Ok(new { payment.Id, payment.Status });
    }

    // GET internal/payments/order/{orderId}
    [HttpGet("order/{orderId:guid}")]
    public async Task<IActionResult> GetByOrderId(Guid orderId)
    {
        if (!IsAuthorized()) return Denied();
        var payment = await _db.Payments.AsNoTracking().FirstOrDefaultAsync(p => p.OrderId == orderId);
        if (payment == null) return NotFound();
        return Ok(new { payment.Id, payment.OrderId, payment.Method, payment.Status, payment.Amount });
    }

    public record ProcessPaymentRequest(Guid OrderId, string Method, decimal Amount);
}
