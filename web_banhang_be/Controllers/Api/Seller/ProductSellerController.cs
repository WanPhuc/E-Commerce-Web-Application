using Microsoft.AspNetCore.Mvc;
using WebBanHang.Models.Common;
using WebBanHang.Models.DTOs.Seller.Product;
using WebBanHang.Models.DTOs.Seller.ProductImage;
using WebBanHang.Models.DTOs.Sellers.Product;
using WebBanHang.Services.Interfaces;
using WebBanHang.Filters;
using WebBanHang.Controllers.Api.Base;

namespace WebBanHang.Controllers.Api.Seller;

[ApiController]
[Route("api/v1/rseller/products")]
[AuthorizeRole("Seller")]
public class ProductSellerController : BaseController
{
    private readonly IProductSellerService _productSellerService;
    public ProductSellerController(IProductSellerService productSellerService)
    {
        _productSellerService = productSellerService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAllProducts([FromQuery] PagedRequest request)
    {
        
        var products = await _productSellerService.GetAllProductsAsync(request);
        return BaseResult(products);
    }
    [HttpGet("detail/{productId:guid}")]
    public async Task<IActionResult> GetProductById(Guid productId)
    {
        var product = await _productSellerService.GetProductByIdAsync(productId);
        return BaseResult(product);
    }
    [HttpPost("create")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateSellerProductDto dto)
    {
        var product = await _productSellerService.CreateProductAsync(dto);
        return BaseResult(product);
    }
    [HttpPut("update/{productId:guid}")]
    public async Task<IActionResult> UpdateProduct(Guid productId, [FromBody] UpdateSellerProductDto dto)
    {
        var product = await _productSellerService.UpdateProductAsync(productId, dto);
        return BaseResult(product);
    }
    [HttpDelete("delete/{productId:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid productId)
    {
        var result = await _productSellerService.DeleteProductAsync(productId);
        return BaseResult(result);
    }
    [HttpPut("change-status/{productId:guid}")]
    public async Task<IActionResult> ChangeProductStatus(Guid productId, ProductStatus newStatus)
    {
        var result = await _productSellerService.ChangeProductStatusAsync(productId, newStatus);
        return BaseResult(result);
    }


    //////////////#ProductImage///////////////////
    [HttpPost("{productId:guid}/images")]
    public async Task<IActionResult> AddProductImage(Guid productId, [FromBody] ProductImageCreateDto dto)
    {
        var image = await _productSellerService.AddProductImageAsync(productId, dto);
        return BaseResult(image);
    }
    [HttpPut("{productId:guid}/images/{imageId:guid}")]
    public async Task<IActionResult> UpdateProductImage(Guid productId, Guid imageId, [FromBody] ProductImageCreateDto dto)
    {
        var image = await _productSellerService.UpdateProductImageAsync(productId, imageId, dto);
        return BaseResult(image);
    }
    [HttpDelete("{productId:guid}/images/{imageId:guid}")]
    public async Task<IActionResult> DeleteProductImage(Guid productId, Guid imageId)
    {
        var result = await _productSellerService.DeleteProductImageAsync(productId, imageId);
        return BaseResult(result);
    }
    [HttpPut("{productId:guid}/images/{imageId:guid}/set-main")]
    public async Task<IActionResult> SetMainProductImage(Guid productId, Guid imageId)
    {
        var result = await _productSellerService.SetMainProductImageAsync(productId, imageId);
        return BaseResult(result);
    }

}
