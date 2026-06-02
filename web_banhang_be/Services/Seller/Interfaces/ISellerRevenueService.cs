using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Sellers.Revenue;
using WebBanHang.Models.Enums;

namespace WebBanHang.Services.Seller.Interfaces;
public interface ISellerRevenueService
{
    Task<ApiResponse<SellerRevenueDto>> GetSellerRevenueAsync(CancellationToken ct=default);
    Task<ApiResponse<SellerRevenueChartDto>> GetSellerRevenueChartAsync(  ChartRanger range,CancellationToken ct=default);
    Task<ApiResponse<PagedResult<SellerTopSellingProductsDto>>> GetSellerTopSellingProductsAsync(PagedRequest request, CancellationToken ct=default);

}
