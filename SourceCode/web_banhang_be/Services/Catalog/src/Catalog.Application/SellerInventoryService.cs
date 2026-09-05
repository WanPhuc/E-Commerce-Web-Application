using WebBanHang.Helpers.Product;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Extensions;

using AuraMart.Catalog.Dtos;


using AuraMart.Catalog.Services;
namespace AuraMart.Catalog.Services;
public class SellerInventoryService : ISellerInventoryService
{
    private readonly IProductRepository _productRepository;
    private readonly ISellerLookup _sellerLookup;
    private readonly ICommonService _commonService;
    public SellerInventoryService(IProductRepository productRepository, ISellerLookup sellerLookup, ICommonService commonService)
    {
        _productRepository = productRepository;
        _sellerLookup = sellerLookup;
        _commonService = commonService;
    }
    public async Task<ApiResponse<PagedResult<SellerInventoryDto>>> GetSellerInventoryAsync(PagedRequest request, string? filter = null)
    {
        var seller = await _sellerLookup.FindByUserIdAsync(_commonService.GetUserId());
        if (seller == null) return ApiResponse<PagedResult<SellerInventoryDto>>.Fail("Seller not found.", 404, ErrorCodes.Seller.NotFound);
        var productsQuery = _productRepository.FindByCondition(p => p.SellerId == seller.Id);
        productsQuery = filter?.ToLower() switch
        {
            "lowstock" => productsQuery.Where(p => p.Stock <= p.LowStockThreshold && p.Stock > 0),
            "outofstock" => productsQuery.Where(p => p.Stock == 0),
            _ => productsQuery
        };

        var productDtos = await productsQuery
            .OrderBy(p => p.Stock)
            .Select(p => new SellerInventoryDto
            {
                Id = p.Id,
                Name = p.Name,
                SKU = p.SKU,
                Stock = p.Stock,
                LowStockThreshold = p.LowStockThreshold,
                Status = p.Status,
                ImageUrl = p.Images.OrderByDescending(i => i.IsMainImage).Select(i => i.ImageUrl).FirstOrDefault(),
                Price = p.Price,
            })
            .ToPagedAsync(request);
        return ApiResponse<PagedResult<SellerInventoryDto>>.Success(productDtos, "Success", 200, SuccessCodes.Seller.InventoryRetrieved);
    }
    public async Task<ApiResponse<string>> UpdateSellerInventoryAsync(Guid productId, UpdateSellerInventoryDto updateDto)
    {
        var seller = await _sellerLookup.FindByUserIdAsync(_commonService.GetUserId());
        if (seller == null) return ApiResponse<string>.Fail("Seller not found.", 404, ErrorCodes.Seller.NotFound);

        var product = await _productRepository.FindSingleByConditionAsync(
            p => p.Id == productId && p.SellerId == seller.Id && !p.DeleteFlg,
            true);
        if (product == null) return ApiResponse<string>.Fail("Product not found.", 404, ErrorCodes.Product.NotFound);

        product.Stock = updateDto.Stock ?? product.Stock;
        product.LowStockThreshold = updateDto.LowStockThreshold ?? product.LowStockThreshold;
        product.Status = updateDto.Status ?? product.Status;

        await _productRepository.UpdateAsync(product);
        return ApiResponse<string>.Success("Inventory updated successfully.", "Inventory updated successfully.", 200, SuccessCodes.Seller.InventoryUpdated);
    }
}
