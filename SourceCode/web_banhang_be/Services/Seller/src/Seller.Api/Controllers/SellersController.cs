using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuraMart.Seller.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SellersController : BaseController
{
    private readonly ISellerService _sellerService;

    public SellersController(ISellerService sellerService)
    {
        _sellerService = sellerService;
    }

    [HttpPost("apply")]
    [Authorize]
    public async Task<IActionResult> Apply([FromBody] RegisterSellerRequest request)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            return Unauthorized(ApiResponse<object>.Fail("Chưa đăng nhập", 401));
        }

        var result = await _sellerService.ApplyAsync(userId, request);
        return BaseResult(result);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMyProfile()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            return Unauthorized(ApiResponse<object>.Fail("Chưa đăng nhập", 401));
        }

        var result = await _sellerService.GetMySellerProfileAsync(userId);
        return BaseResult(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _sellerService.GetSellerByIdAsync(id);
        return BaseResult(result);
    }

    [HttpGet("applications/pending")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPendingApplications()
    {
        var result = await _sellerService.GetPendingApplicationsAsync();
        return BaseResult(result);
    }

    [HttpPut("applications/{id:guid}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ApproveApplication(Guid id)
    {
        var result = await _sellerService.ApproveApplicationAsync(id);
        return BaseResult(result);
    }
}
