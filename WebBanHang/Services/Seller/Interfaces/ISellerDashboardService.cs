using WebBanHang.Models.DTOs.Sellers;
using WebBanHang.Models.Enums;

namespace WebBanHang.Services.Seller.Interfaces;
public interface ISellerDashboardService
{
    Task<SellerDashboardDto> GetSellerDashboardAsync(Guid sellerId,CancellationToken ct=default);
    Task<SellerDashboardChartDto> GetSellerDashboardChartAsync(Guid sellerId, ChartRanger ranger,CancellationToken ct=default);
}