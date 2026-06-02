using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Controllers.Api.Base;
using WebBanHang.Filters;
using WebBanHang.Models.Common;            // ✅ ApiResponse
using WebBanHang.Models.DTOs.Admin.Sellers;
using WebBanHang.Models.DTOs.Sellers;
using WebBanHang.Services.Interfaces;

namespace WebBanHang.Controllers.Api.Admin;

[ApiController]
[Route("api/v1/admin/sellers")]
[AuthorizeRole("Admin")]
public class SellerController : BaseController
{
    private readonly ISellerService _sellerService;
    private readonly ISellerApplicationService _sellerAppService;

    public SellerController(ISellerService sellerService, ISellerApplicationService sellerAppService)
    {
        _sellerService = sellerService;
        _sellerAppService = sellerAppService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagedRequest request)
    {
        var result = await _sellerService.GetAllSellersAsync(request);
        return BaseResult(result);
    }

    [HttpPost("{applicationId:guid}/approved")]
    public async Task<IActionResult> ApproveSellerApplication(Guid applicationId)
    {
        var result = await _sellerAppService.ApproveSellerApplicationAsync(applicationId);
        return BaseResult(result);
    }

    [HttpPost("{applicationId:guid}/rejected")]
    public async Task<IActionResult> RejectSellerApplication(Guid applicationId)
    {
        var result = await _sellerAppService.RejectSellerApplicationAsync(applicationId);
        return BaseResult(result);
    }

    [HttpGet("sellers/{sellerId:guid}")]
    public async Task<IActionResult> GetSellerDetailById(Guid sellerId)
    {
        var result = await _sellerService.GetSellerDetailByIdAsync(sellerId);
        return BaseResult(result);
    }

    [HttpGet("application-seller/{applicationId:guid}")]
    public async Task<IActionResult> GetSellerApplicationDetailById(Guid applicationId)
    {
        var result = await _sellerAppService.GetSellerApplicationDetailByIdAsync(applicationId);
        return BaseResult(result);
    }
}
