using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Controllers.Api.Base;
using WebBanHang.Filters;
using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Sellers.Revenue;
using WebBanHang.Models.Enums;
using WebBanHang.Repositories.Interfaces;
using WebBanHang.Services.Seller.Interfaces;

namespace WebBanHang.Controllers.Api.Seller;
[ApiController]
[Route("api/v1/rseller/revenue")]
[AuthorizeRole("Seller")]
public class SellerRevenue : BaseController
{
    private readonly ISellerRevenueService _sellerRevenueService;
    private readonly ISellerRepository _sellerRepository;
    public SellerRevenue(ISellerRevenueService sellerRevenueService, ISellerRepository sellerRepository)
    {
        _sellerRepository = sellerRepository;
        _sellerRevenueService = sellerRevenueService;
    }

    [HttpGet]
    public async Task<IActionResult> GetSellerRevenue(CancellationToken ct = default)
    {
        var revenue = await _sellerRevenueService.GetSellerRevenueAsync(ct);
        return BaseResult(revenue);
    }
    [HttpGet("chart")]
    public async Task<IActionResult> GetSellerRevenueChart(ChartRanger range,CancellationToken ct = default)
    { 
        var revenueChart = await _sellerRevenueService.GetSellerRevenueChartAsync( range,ct);
        return BaseResult(revenueChart);
    }
    [HttpGet("top-selling")]
    public async Task<IActionResult> GetSellerTopSellingProducts([FromQuery] PagedRequest request, CancellationToken ct = default)
    {   
        var topSellingProducts = await _sellerRevenueService.GetSellerTopSellingProductsAsync(request, ct);
        return BaseResult(topSellingProducts);
    }

}
