namespace AuraMart.Ordering.Application;

public interface IOrderService
{
    Task<ApiResponse<OrderResponseDto>> CreateOrderAsync(Guid userId, string userName, string userEmail, CreateOrderRequest request);
    Task<ApiResponse<OrderResponseDto?>> GetOrderByIdAsync(Guid orderId, Guid userId);
    Task<ApiResponse<List<OrderResponseDto>>> GetMyOrdersAsync(Guid userId);
    Task<ApiResponse<List<OrderResponseDto>>> GetSellerOrdersAsync(Guid sellerId);
    Task<ApiResponse<bool>> CancelOrderAsync(Guid orderId, Guid userId, string reason);
}
