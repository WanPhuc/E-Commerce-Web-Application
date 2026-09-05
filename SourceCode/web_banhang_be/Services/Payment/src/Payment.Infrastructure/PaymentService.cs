using BuildingBlocks.EventBus;
using BuildingBlocks.Outbox;
using System.Text.Json;

namespace AuraMart.Payment.Infrastructure;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly PaymentDbContext _db;

    public PaymentService(IPaymentRepository paymentRepository, PaymentDbContext db)
    {
        _paymentRepository = paymentRepository;
        _db = db;
    }

    public async Task<ApiResponse<PaymentResponseDto>> CreatePaymentAsync(CreatePaymentRequest request)
    {
        if (request.Amount <= 0)
        {
            return ApiResponse<PaymentResponseDto>.Fail("Số tiền thanh toán không hợp lệ", 400);
        }

        var payment = new AuraMart.Payment.Domain.Payment
        {
            Id = Guid.NewGuid(),
            OrderId = request.OrderId,
            Method = request.Method,
            Amount = request.Amount,
            Status = "Completed",
            CreatedAt = DateTime.UtcNow
        };

        _db.Payments.Add(payment);

        // Outbox event
        var paymentSucceededEvt = new PaymentSucceededIntegrationEvent
        {
            OrderId = request.OrderId,
            PaymentId = payment.Id,
            Method = request.Method,
            Amount = request.Amount
        };

        _db.OutboxMessages.Add(new OutboxMessage
        {
            Id = paymentSucceededEvt.EventId,
            Type = paymentSucceededEvt.GetType().AssemblyQualifiedName ?? paymentSucceededEvt.GetType().Name,
            Payload = JsonSerializer.Serialize(paymentSucceededEvt),
            OccurredOnUtc = paymentSucceededEvt.OccurredOnUtc
        });

        await _db.SaveChangesAsync();

        return ApiResponse<PaymentResponseDto>.Success(MapToDto(payment), "Thanh toán thành công", 201);
    }

    public async Task<ApiResponse<PaymentResponseDto?>> GetPaymentByOrderIdAsync(Guid orderId)
    {
        var payment = await _paymentRepository.GetByOrderIdAsync(orderId);
        if (payment == null)
        {
            return ApiResponse<PaymentResponseDto?>.Fail("Không tìm thấy thông tin thanh toán cho đơn hàng này", 404);
        }
        return ApiResponse<PaymentResponseDto?>.Success(MapToDto(payment));
    }

    public async Task<ApiResponse<PaymentResponseDto?>> GetPaymentByIdAsync(Guid id)
    {
        var payment = await _paymentRepository.GetByIdAsync(id);
        if (payment == null)
        {
            return ApiResponse<PaymentResponseDto?>.Fail("Không tìm thấy giao dịch thanh toán", 404);
        }
        return ApiResponse<PaymentResponseDto?>.Success(MapToDto(payment));
    }

    private static PaymentResponseDto MapToDto(AuraMart.Payment.Domain.Payment p) => new()
    {
        Id = p.Id,
        OrderId = p.OrderId,
        Method = p.Method,
        Amount = p.Amount,
        Status = p.Status,
        CreatedAt = p.CreatedAt
    };
}
