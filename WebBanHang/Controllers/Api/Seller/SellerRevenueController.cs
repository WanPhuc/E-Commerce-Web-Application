using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Sellers.Revenue;
using WebBanHang.Models.Enums;
using WebBanHang.Repositories;
using WebBanHang.Services.Seller.Interfaces;

namespace WebBanHang.Controllers.Api.Seller;
[ApiController]
[Route("api/v1/rseller/revenue")]
[Authorize(Roles = "Seller")]
public class SellerRevenue : ControllerBase
{
    private readonly ISellerRevenueService _sellerRevenueService;
    private readonly ISellerRepository _sellerRepository;
    public SellerRevenue(ISellerRevenueService sellerRevenueService, ISellerRepository sellerRepository)
    {
        _sellerRepository = sellerRepository;
        _sellerRevenueService = sellerRevenueService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<SellerRevenueDto>>> GetSellerRevenue(CancellationToken ct = default)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized("Not found user information.");
        Guid userId = Guid.Parse(userIdClaim.Value);
        var sellerId= await _sellerRepository.GetSellerByUserIdAsync(userId);
        if(sellerId == null) return NotFound(ApiResponse<SellerRevenueDto>.Fail("Seller not found.",404));
        var revenue = await _sellerRevenueService.GetSellerRevenueAsync(sellerId.Id,ct);
        return Ok(ApiResponse<SellerRevenueDto>.Ok(revenue));
    }
    [HttpGet("chart")]
    public async Task<ActionResult<ApiResponse<SellerRevenueChartDto>>> GetSellerRevenueChart(ChartRanger range,CancellationToken ct = default)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized("Not found user information.");
        Guid userId = Guid.Parse(userIdClaim.Value);
        var sellerId= await _sellerRepository.GetSellerByUserIdAsync(userId);
        if(sellerId == null) return NotFound(ApiResponse<SellerRevenueChartDto>.Fail("Seller not found.",404));
        var revenueChart = await _sellerRevenueService.GetSellerRevenueChartAsync(sellerId.Id, range,ct);
        return Ok(ApiResponse<SellerRevenueChartDto>.Ok(revenueChart));
    }
    [HttpGet("top-selling")]
    public async Task<ActionResult<ApiResponse<List<SellerTopSellingProductsDto>>>> GetSellerTopSellingProducts(int topN,CancellationToken ct = default)
    {   
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized("Not found user information.");
        Guid userId = Guid.Parse(userIdClaim.Value);
        var sellerId= await _sellerRepository.GetSellerByUserIdAsync(userId);
        if(sellerId == null) return NotFound(ApiResponse<List<SellerTopSellingProductsDto>>.Fail("Seller not found.",404));
        var topSellingProducts = await _sellerRevenueService.GetSellerTopSellingProductsAsync(sellerId.Id, topN,ct);
        return Ok(ApiResponse<List<SellerTopSellingProductsDto>>.Ok(topSellingProducts));
    }

}