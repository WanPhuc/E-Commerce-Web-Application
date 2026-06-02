using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Sellers;
using WebBanHang.Models.Enums;

namespace WebBanHang.Services.Seller.Interfaces;
public interface ISellerDashboardService
{
    Task<ApiResponse<SellerDashboardDto>> GetSellerDashboardAsync(CancellationToken ct=default);
    Task<ApiResponse<SellerDashboardChartDto>> GetSellerDashboardChartAsync( ChartRanger ranger,CancellationToken ct=default);
}
