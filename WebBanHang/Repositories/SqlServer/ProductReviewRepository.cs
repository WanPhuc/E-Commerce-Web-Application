using Microsoft.EntityFrameworkCore;
using WebBanHang.Data;
using WebBanHang.Models;

namespace WebBanHang.Repositories.SqlServer;
public class ProductReviewRepository : SqlServerRepository<ProductReview>, IProductReviewRepository
{
    public ProductReviewRepository(AppDbContext context) : base(context)
    {
    }

    protected override IQueryable<ProductReview> Query => _dbSet
        .AsNoTracking()
        .Include(r=>r.User)
        .Include(r=>r.Product);

    public async Task<List<ProductReview>> GetReviewsByProductIdAsync(Guid productId, CancellationToken ct = default)
    {
        return await Query.Where(r => r.ProductId == productId).ToListAsync(ct);
    }

    public async Task<double> GetAverageRatingByProductIdAsync(Guid productId, CancellationToken ct = default)
    {
        return await Query.Where(r => r.ProductId == productId).AverageAsync(r => (double?)r.Rating, ct)??0;
    }

    public async Task<List<ProductReview>> GetRecentReviewsBySellerIdAsync(Guid sellerId,int limit = 5, CancellationToken ct = default)
    {
        return await Query.Where(r => r.Product.SellerId == sellerId).OrderByDescending(r => r.CreatedAt).Take(limit).ToListAsync(ct);
    }

    public async Task<double> GetAverageRatingBySellerIdAsync(Guid sellerId, CancellationToken ct = default)
    {
        return await Query.Where(r => r.Product.SellerId == sellerId).AverageAsync(r => (double?)r.Rating, ct)?? 0;
    }
}