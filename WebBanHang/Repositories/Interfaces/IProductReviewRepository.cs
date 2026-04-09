using WebBanHang.Models.EntityModels;

namespace WebBanHang.Repositories.Interfaces;
public interface IProductReviewRepository:IRepository<ProductReview>
{
    Task<List<ProductReview>> GetReviewsByProductIdAsync(Guid productId, CancellationToken ct = default);
    Task<double> GetAverageRatingByProductIdAsync(Guid productId, CancellationToken ct = default);
    Task<List<ProductReview>> GetRecentReviewsBySellerIdAsync(Guid sellerId,int limit = 5, CancellationToken ct = default);
    Task<double> GetAverageRatingBySellerIdAsync(Guid sellerId, CancellationToken ct = default);
}