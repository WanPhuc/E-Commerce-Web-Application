using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Controllers.Api.Base;
using WebBanHang.Filters;
using WebBanHang.Models.Common;
namespace WebBanHang.Controllers.Api.Admin;
[ApiController]
[Route("api/v1/admin")]
[AuthorizeRole("Admin")]
public class DashboardController : BaseController
{
    private readonly IAdminService _adminService;
    public DashboardController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboardStats()
    {
        var stats = await _adminService.GetDashboardStatsAsync();
        return BaseResult(stats);
    }
    [HttpGet("chart")]
    public async Task<IActionResult> GetChart(DashboardRanger range)
    {
        var chart = await _adminService.GetChartAsync(range);
        return BaseResult(chart);
    }

}
