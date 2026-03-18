using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Models.Common;            // ✅ ApiResponse
using WebBanHang.Models.DTOs.Sellers;
using WebBanHang.Models.ViewModels;
using WebBanHang.Services.Interfaces;

namespace WebBanHang.Controllers.Api.Admin;

[ApiController]
[Route("api/v1/admin/sellers")]
[Authorize(Roles = "Admin")]
public class SellerController : ControllerBase
{
    private readonly ISellerService _sellerService;
    private readonly ISellerApplicationService _sellerAppService;

    public SellerController(ISellerService sellerService, ISellerApplicationService sellerAppService)
    {
        _sellerService = sellerService;
        _sellerAppService = sellerAppService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<SellerManagementVM>>> GetAll()
    {
        var result = await _sellerService.GetAllSellersAsync();
        return Ok(new ApiResponse<SellerManagementVM>
        {
            Status = 200,
            Message = "Success",
            Data = result
        });
    }

    [HttpPost("{applicationId:guid}/approved")]
    public async Task<ActionResult<ApiResponse<object?>>> ApproveSellerApplication(Guid applicationId)
    {
        try
        {
            await _sellerAppService.ApproveSellerApplicationAsync(applicationId);
            return Ok(new ApiResponse<object?>
            {
                Status = 200,
                Message = "Seller application approved successfully.",
                Data = null
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<object?>
            {
                Status = 404,
                Message = ex.Message,
                Data = null
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse<object?>
            {
                Status = 400,
                Message = ex.Message,
                Data = null
            });
        }
    }

    [HttpPost("{applicationId:guid}/rejected")]
    public async Task<ActionResult<ApiResponse<object?>>> RejectSellerApplication(Guid applicationId)
    {
        try
        {
            await _sellerAppService.RejectSellerApplicationAsync(applicationId);
            return Ok(new ApiResponse<object?>
            {
                Status = 200,
                Message = "Seller application rejected successfully.",
                Data = null
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<object?>
            {
                Status = 404,
                Message = ex.Message,
                Data = null
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse<object?>
            {
                Status = 400,
                Message = ex.Message,
                Data = null
            });
        }
    }

    [HttpGet("sellers/{sellerId:guid}")]
    public async Task<ActionResult<ApiResponse<SellerDetailDto>>> GetSellerDetailById(Guid sellerId)
    {
        try
        {
            var result = await _sellerService.GetSellerDetailByIdAsync(sellerId);
            return Ok(new ApiResponse<SellerDetailDto>
            {
                Status = 200,
                Message = "Success",
                Data = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<SellerDetailDto>
            {
                Status = 404,
                Message = ex.Message,
                Data = null!
            });
        }
    }

    [HttpGet("application-seller/{applicationId:guid}")]
    public async Task<ActionResult<ApiResponse<SellerApplicationDetailDto>>> GetSellerApplicationDetailById(Guid applicationId)
    {
        try
        {
            var result = await _sellerAppService.GetSellerApplicationDetailByIdAsync(applicationId);
            return Ok(new ApiResponse<SellerApplicationDetailDto>
            {
                Status = 200,
                Message = "Success",
                Data = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<SellerApplicationDetailDto>
            {
                Status = 404,
                Message = ex.Message,
                Data = null!
            });
        }
    }
}
