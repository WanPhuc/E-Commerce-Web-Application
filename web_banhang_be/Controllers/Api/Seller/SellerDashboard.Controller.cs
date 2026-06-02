using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Controllers.Api.Base;
using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Sellers;
using WebBanHang.Services.Seller.Interfaces;
using System.Security.Claims;
using WebBanHang.Models.Enums;
using WebBanHang.Repositories.Interfaces;
using WebBanHang.Filters;

namespace WebBanHang.Controllers.Api.Seller;
[ApiController]
[Route("api/v1/rseller/dashboard")]
[AuthorizeRole("Seller")]
public class SellerDashboardController : BaseController
{
    private readonly ISellerDashboardService _sellerDashboardService;
    private readonly ISellerRepository _sellerRepository;
    public SellerDashboardController(ISellerDashboardService sellerDashboardService, ISellerRepository sellerRepository)
    {
        _sellerDashboardService = sellerDashboardService;
        _sellerRepository = sellerRepository;
    }
    [HttpGet]
    public async Task<IActionResult> GetSellerDashboard()
    {
        var dashboardData = await _sellerDashboardService.GetSellerDashboardAsync();
        return BaseResult(dashboardData);
    }
    [HttpGet("chartdashboard")]
    public async Task<IActionResult> GetSellerDashboardChart(ChartRanger ranger)
    {
        var dashboardChartData = await _sellerDashboardService.GetSellerDashboardChartAsync(ranger);
        return BaseResult(dashboardChartData);
    }
}
    
