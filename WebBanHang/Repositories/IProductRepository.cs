using  System.Linq.Expressions;
using WebBanHang.Models;
namespace WebBanHang.Repositories;

public interface IProductRepository : IRepository<Product>
{
    //Admin
    // Task<(List<Product> Items,int Total) >AdminQueryAsync(AdminProductQuery q,CancellationToken ct =default);
    // Task UpdateStatusAsync(Guid productId,ProductStatus status,CancellationToken ct=default);
    // Task SoftDeleteAsync(Guid productId,CancellationToken ct=default);
    //-----------------------------//
    Task<List<Product>> GetAllProductsAsync(CancellationToken ct=default);
    Task<Product?> GetProductsDetailsByIdAsync(Guid id,CancellationToken ct=default);
    Task<List<Product>> GetProductsByCategoryAsync(Guid categoryId,CancellationToken ct=default);
    Task<List<Product>> GetProductsBySellerAsync(Guid sellerId,CancellationToken ct=default);
    Task<List<Product>> GetProductsBySellerWithReviewsAsync(Guid sellerId,CancellationToken ct=default);
    IQueryable<Product> GetQueryableProductsBySeller(Guid sellerId);
    Task<List<Product>> SearchProductsAsync(string searchTerm,CancellationToken ct = default);
    Task<Product?> GetProductBySKUAsync(string SKU,CancellationToken ct=default);
 

}