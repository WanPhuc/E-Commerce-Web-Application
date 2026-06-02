using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Admin.Sellers;
using WebBanHang.Models.DTOs.Sellers;

namespace WebBanHang.Services.Interfaces;

public interface ISellerService
{
    Task<ApiResponse<SellerManagementVM>> GetAllSellersAsync(PagedRequest request);
    Task<ApiResponse<SellerDetailDto>> GetSellerDetailByIdAsync(Guid sellerId);
}
