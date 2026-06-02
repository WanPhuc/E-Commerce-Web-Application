using WebBanHang.Models.Common;

public interface IAdminService
{
    Task<ApiResponse<AdminDashboardDto>> GetDashboardStatsAsync(CancellationToken ct = default);
    Task<ApiResponse<DashboardChartDto>>GetChartAsync(DashboardRanger range,CancellationToken ct=default);
}
