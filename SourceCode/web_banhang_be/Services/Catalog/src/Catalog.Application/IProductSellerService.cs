
using AuraMart.Catalog.Dtos;
namespace AuraMart.Catalog.Services;
public interface IProductSellerService
{
    Task<ApiResponse<PagedResult<SellerProductDto>>> GetAllProductsAsync(PagedRequest request);
    Task<ApiResponse<SellerProductDto>> GetProductByIdAsync(Guid productId);
    Task<ApiResponse<SellerProductDto>> CreateProductAsync(CreateSellerProductDto createSellerProductDto);
    Task<ApiResponse<SellerProductDto>> UpdateProductAsync(Guid productId, UpdateSellerProductDto dto);
    Task<ApiResponse<string>> DeleteProductAsync(Guid productId);
    Task<ApiResponse<string>> ChangeProductStatusAsync(Guid productId,ProductStatus newStatus);

    //Product Image
    Task<ApiResponse<ProductImageDto>> AddProductImageAsync(Guid productId,ProductImageCreateDto dto);
    Task<ApiResponse<ProductImageDto>> UpdateProductImageAsync(Guid productId,Guid imageId,ProductImageCreateDto dto);
    Task<ApiResponse<string>> DeleteProductImageAsync(Guid productId,Guid imageId);
    Task<ApiResponse<bool>> SetMainProductImageAsync(Guid productId,Guid imageId);
}
