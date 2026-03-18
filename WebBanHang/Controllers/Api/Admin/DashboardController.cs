using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Models.Common;
namespace WebBanHang.Controllers.Api.Admin;
[ApiController]
[Route("api/v1/admin")]
[Authorize(Roles = "Admin")]
public class DashboardController : ControllerBase
{
    private readonly IAdminService _adminService;
    public DashboardController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<AdminDashboardDto>>> GetDashboardStats()
    {
        var stats = await _adminService.GetDashboardStatsAsync();
        return Ok(ApiResponse<AdminDashboardDto>.Ok(stats));
    }
    [HttpGet("chart")]
    public async Task<ActionResult<ApiResponse<DashboardChartDto>>> GetChart(DashboardRanger range)
    {
        var chart = await _adminService.GetChartAsync(range);
        return Ok(ApiResponse<DashboardChartDto>.Ok(chart));
    }

}