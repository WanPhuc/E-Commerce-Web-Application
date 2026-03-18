public interface IAdminService
{
    Task<AdminDashboardDto> GetDashboardStatsAsync(CancellationToken ct = default);
    Task<DashboardChartDto>GetChartAsync(DashboardRanger range,CancellationToken ct=default);
}