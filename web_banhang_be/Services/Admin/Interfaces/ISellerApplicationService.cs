using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Sellers;

namespace WebBanHang.Services.Interfaces;

public interface ISellerApplicationService
{
    Task<ApiResponse<SellerApplicationDetailDto>> GetSellerApplicationDetailByIdAsync(Guid applicationId);
    Task<ApiResponse<object?>> ApproveSellerApplicationAsync(Guid applicationId);
    Task<ApiResponse<object?>> RejectSellerApplicationAsync(Guid applicationId);
}
