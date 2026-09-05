namespace AuraMart.Cart.Application;

public interface ICartService
{
    Task<ApiResponse<CartDto>> GetCartAsync(Guid userId);
    Task<ApiResponse<CartDto>> AddItemAsync(Guid userId, AddToCartRequest request);
    Task<ApiResponse<CartDto>> UpdateItemAsync(Guid userId, Guid cartItemId, UpdateCartItemRequest request);
    Task<ApiResponse<CartDto>> RemoveItemAsync(Guid userId, Guid cartItemId);
    Task<ApiResponse<bool>> ClearCartAsync(Guid userId);
}
