namespace AuraMart.Seller.Application;

public interface ISellerService
{
    Task<ApiResponse<SellerApplicationResponseDto>> ApplyAsync(Guid userId, RegisterSellerRequest request);
    Task<ApiResponse<SellerResponseDto?>> GetMySellerProfileAsync(Guid userId);
    Task<ApiResponse<SellerResponseDto?>> GetSellerByIdAsync(Guid id);
    Task<ApiResponse<List<SellerApplicationResponseDto>>> GetPendingApplicationsAsync();
    Task<ApiResponse<bool>> ApproveApplicationAsync(Guid applicationId);
}
