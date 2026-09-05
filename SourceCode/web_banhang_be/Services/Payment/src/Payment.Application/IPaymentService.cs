namespace AuraMart.Payment.Application;

public interface IPaymentService
{
    Task<ApiResponse<PaymentResponseDto>> CreatePaymentAsync(CreatePaymentRequest request);
    Task<ApiResponse<PaymentResponseDto?>> GetPaymentByOrderIdAsync(Guid orderId);
    Task<ApiResponse<PaymentResponseDto?>> GetPaymentByIdAsync(Guid id);
}
