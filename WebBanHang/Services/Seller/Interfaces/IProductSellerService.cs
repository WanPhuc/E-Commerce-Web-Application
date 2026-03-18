using WebBanHang.Models.DTOs.Seller.Product;
using WebBanHang.Models.DTOs.Seller.ProductImage;
using WebBanHang.Models.DTOs.Sellers.Product;

namespace WebBanHang.Services.Interfaces;
public interface IProductSellerService
{
    Task<List<SellerProductDto>> GetAllProductsAsync(Guid userId);
    Task<SellerProductDto> GetProductByIdAsync(Guid productId);
    Task<SellerProductDto> CreateProductAsync(Guid userId,CreateSellerProductDto createSellerProductDto);
    Task<SellerProductDto> UpdateProductAsync(Guid userId,Guid productId, UpdateSellerProductDto dto);
    Task DeleteProductAsync(Guid userId,Guid productId);
    Task<string> ChangeProductStatusAsync(Guid userId,Guid productId,ProductStatus newStatus);

    //Product Image
    Task<ProductImageDto> AddProductImageAsync(Guid userId,Guid productId,ProductImageCreateDto dto);
    Task<ProductImageDto> UpdateProductImageAsync(Guid userId,Guid productId,Guid imageId,ProductImageUpdateDto dto);
    Task DeleteProductImageAsync(Guid userId,Guid productId,Guid imageId);
    Task SetMainProductImageAsync(Guid userId,Guid productId,Guid imageId);
}