using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Sellers;
using WebBanHang.Services.Seller.Interfaces;
using System.Security.Claims;
using WebBanHang.Repositories;
using WebBanHang.Models.Enums;

namespace WebBanHang.Controllers.Api.Seller;
[ApiController]
[Route("api/v1/rseller/dashboard")]
[Authorize(Roles = "Seller")]
public class SellerDashboardController : ControllerBase
{
    private readonly ISellerDashboardService _sellerDashboardService;
    private readonly ISellerRepository _sellerRepository;
    public SellerDashboardController(ISellerDashboardService sellerDashboardService, ISellerRepository sellerRepository)
    {
        _sellerDashboardService = sellerDashboardService;
        _sellerRepository = sellerRepository;
    }
    [HttpGet]
    public async Task<ActionResult<ApiResponse<SellerDashboardDto>>> GetSellerDashboard()
    {        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized("Not found user information.");
            Guid userId = Guid.Parse(userIdClaim.Value);
            var seller = await _sellerRepository.GetSellerByUserIdAsync(userId);
            if (seller == null) return NotFound(ApiResponse<SellerDashboardDto>.Fail("Seller not found.", 404));
            var dashboardData = await _sellerDashboardService.GetSellerDashboardAsync(seller.Id);
            return Ok(ApiResponse<SellerDashboardDto>.Ok(dashboardData));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<SellerDashboardDto>.Fail(ex.Message, 404));
        }
    }
    [HttpGet("chartdashboard")]
    public async Task<ActionResult<ApiResponse<SellerDashboardChartDto>>> GetSellerDashboardChart(ChartRanger ranger)
    {        
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized("Not found user information.");
            Guid userId = Guid.Parse(userIdClaim.Value);
            var seller = await _sellerRepository.GetSellerByUserIdAsync(userId);
            if (seller == null) return NotFound(ApiResponse<SellerDashboardChartDto>.Fail("Seller not found.", 404));
            var dashboardChartData = await _sellerDashboardService.GetSellerDashboardChartAsync(seller.Id, ranger);
            return Ok(ApiResponse<SellerDashboardChartDto>.Ok(dashboardChartData));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<SellerDashboardChartDto>.Fail(ex.Message, 404));
        }
    }
}
    