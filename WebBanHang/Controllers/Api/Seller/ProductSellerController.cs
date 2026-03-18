using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Seller.Product;
using WebBanHang.Models.DTOs.Seller.ProductImage;
using WebBanHang.Models.DTOs.Sellers.Product;
using WebBanHang.Services.Interfaces;
using System.Security.Claims;

namespace WebBanHang.Controllers.Api.Seller;

[ApiController]
[Route("api/v1/rseller/products")]
[Authorize(Roles = "Seller")]
public class ProductSellerController : ControllerBase
{
    private readonly IProductSellerService _productSellerService;
    public ProductSellerController(IProductSellerService productSellerService)
    {
        _productSellerService = productSellerService;
    }
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<SellerProductDto>>>> GetAllProducts()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized("Not found user information.");
        Guid userId = Guid.Parse(userIdClaim.Value);
        var products = await _productSellerService.GetAllProductsAsync(userId);
        return Ok(ApiResponse<List<SellerProductDto>>.Ok(products));
    }
    [HttpGet("detail/{productId:guid}")]
    public async Task<ActionResult<ApiResponse<SellerProductDto>>> GetProductById(Guid productId)
    {
        try
        {
            var product = await _productSellerService.GetProductByIdAsync(productId);
            return Ok(ApiResponse<SellerProductDto>.Ok(product));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<SellerProductDto>.Fail(ex.Message, 404));
        }
    }
    [HttpPost("create")]
    public async Task<ActionResult<ApiResponse<SellerProductDto>>> CreateProduct([FromBody] CreateSellerProductDto dto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized("Not found user information.");
            Guid userId = Guid.Parse(userIdClaim.Value);
            var product = await _productSellerService.CreateProductAsync(userId, dto);
            return Ok(ApiResponse<SellerProductDto>.Ok(product, "Product created successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<SellerProductDto>.Fail(ex.Message, 404));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<SellerProductDto>.Fail(ex.Message, 400));
        }

    }
    [HttpPut("update/{productId:guid}")]
    public async Task<ActionResult<ApiResponse<SellerProductDto>>> UpdateProduct(Guid productId, [FromBody] UpdateSellerProductDto dto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized("Not found user information.");
            Guid userId = Guid.Parse(userIdClaim.Value);
            var product = await _productSellerService.UpdateProductAsync(userId, productId, dto);
            return Ok(ApiResponse<SellerProductDto>.Ok(product, "Product updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<SellerProductDto>.Fail(ex.Message, 404));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<SellerProductDto>.Fail(ex.Message, 400));
        }
    }
    [HttpDelete("delete/{productId:guid}")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteProduct(Guid productId)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized("Not found user information.");
            Guid userId = Guid.Parse(userIdClaim.Value);
            await _productSellerService.DeleteProductAsync(userId, productId);
            return Ok(ApiResponse<string>.Ok("Product deleted successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<string>.Fail(ex.Message, 404));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message, 400));
        }
    }
    [HttpPut("change-status/{productId:guid}")]
    public async Task<ActionResult<ApiResponse<string>>> ChangeProductStatus(Guid productId, ProductStatus newStatus)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized("Not found user information.");
            Guid userId = Guid.Parse(userIdClaim.Value);
            var result = await _productSellerService.ChangeProductStatusAsync(userId, productId, newStatus);
            return Ok(ApiResponse<string>.Ok(result, "Product status changed successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<string>.Fail(ex.Message, 404));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message, 400));
        }
    }


    //////////////#ProductImage///////////////////
    [HttpPost("{productId:guid}/images")]
    public async Task<ActionResult<ApiResponse<ProductImageDto>>> AddProductImage(Guid productId, [FromBody] ProductImageCreateDto dto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized("Not found user information.");
            Guid userId = Guid.Parse(userIdClaim.Value);
            var image = await _productSellerService.AddProductImageAsync(userId, productId, dto);
            return Ok(ApiResponse<ProductImageDto>.Ok(image, "Product image added successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<ProductImageDto>.Fail(ex.Message, 404));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<ProductImageDto>.Fail(ex.Message, 400));
        }
    }
    [HttpPut("{productId:guid}/images/{imageId:guid}")]
    public async Task<ActionResult<ApiResponse<ProductImageDto>>> UpdateProductImage(Guid productId, Guid imageId, [FromBody] ProductImageUpdateDto dto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized("Not found user information.");
            Guid userId = Guid.Parse(userIdClaim.Value);
            var image = await _productSellerService.UpdateProductImageAsync(userId, productId, imageId, dto);
            return Ok(ApiResponse<ProductImageDto>.Ok(image, "Product image updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<ProductImageDto>.Fail(ex.Message, 404));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<ProductImageDto>.Fail(ex.Message, 400));
        }
    }
    [HttpDelete("{productId:guid}/images/{imageId:guid}")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteProductImage(Guid productId, Guid imageId)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized("Not found user information.");
            Guid userId = Guid.Parse(userIdClaim.Value);
            await _productSellerService.DeleteProductImageAsync(userId, productId, imageId);
            return Ok(ApiResponse<string>.Ok("Product image deleted successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<string>.Fail(ex.Message, 404));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message, 400));
        }
    }
    [HttpPut("{productId:guid}/images/{imageId:guid}/set-main")]
    public async Task<ActionResult<ApiResponse<bool>>> SetMainProductImage(Guid productId, Guid imageId)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized("Not found user information.");
            Guid userId = Guid.Parse(userIdClaim.Value);
            await _productSellerService.SetMainProductImageAsync(userId, productId, imageId);
            return Ok(ApiResponse<bool>.Ok(true, "Main product image set successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<bool>.Fail(ex.Message, 404));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<bool>.Fail(ex.Message, 400));
        }
    }

}