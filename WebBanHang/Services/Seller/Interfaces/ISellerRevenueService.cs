using WebBanHang.Models.DTOs.Sellers.Revenue;
using WebBanHang.Models.Enums;

namespace WebBanHang.Services.Seller.Interfaces;
public interface ISellerRevenueService
{
    Task<SellerRevenueDto> GetSellerRevenueAsync(Guid sellerId,CancellationToken ct=default);
    Task<SellerRevenueChartDto> GetSellerRevenueChartAsync(Guid sellerId, ChartRanger range,CancellationToken ct=default);
    Task<List<SellerTopSellingProductsDto>> GetSellerTopSellingProductsAsync(Guid sellerId, int topN ,CancellationToken ct=default);

}