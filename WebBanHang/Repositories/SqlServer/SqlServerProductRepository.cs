using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebBanHang.Data;
using WebBanHang.Models;
namespace WebBanHang.Repositories;

public class SqlServerProductRepository : SqlServerRepository<Product>, IProductRepository
{
    public SqlServerProductRepository(AppDbContext context): base(context){}
    // Query nhẹ cho list — không include Reviews
    protected override IQueryable<Product> Query => _dbSet
        .AsNoTracking()
        .Include(p => p.Category)
        .Include(p => p.Seller)
        .Include(p => p.Images);

    // Query đầy đủ chỉ dùng cho detail
    private IQueryable<Product> DetailQuery => Query
        .Include(p => p.Reviews);
    
    public async Task<List<Product>> GetAllProductsAsync(CancellationToken ct=default)
    {
        return await Query.ToListAsync(ct);
    }
   
    public override async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product?> GetProductsDetailsByIdAsync(Guid id,CancellationToken ct=default)
    {
        return await DetailQuery.FirstOrDefaultAsync(p => p.Id == id,ct);
    }
    public async Task<List<Product>> GetProductsByCategoryAsync(Guid categoryId,CancellationToken ct=default)
    {
        return await Query.Where(p => p.CategoryId == categoryId).ToListAsync(ct);
    
    }
    //seller
    public async Task<List<Product>> GetProductsBySellerAsync(Guid sellerId,CancellationToken ct=default)
    {
        return await Query.Where(p=>p.SellerId==sellerId).ToListAsync(ct);
    }
     public async Task<List<Product>> GetProductsBySellerWithReviewsAsync(Guid sellerId, CancellationToken ct = default)
    {
        return await DetailQuery 
            .Where(p => p.SellerId == sellerId)
            .ToListAsync(ct);
    }
    public IQueryable<Product> GetQueryableProductsBySeller(Guid sellerId)
    {
        return Query.Where(p => p.SellerId == sellerId);
    }
    public async Task<List<Product>> SearchProductsAsync(string searchTerm,CancellationToken ct=default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await Query.ToListAsync(ct);
        }
        searchTerm = searchTerm.Trim().ToLower();
        var pattern = $"%{searchTerm}%";
        return await Query.Where(p =>
            EF.Functions.Like(p.Name, pattern) ||
            EF.Functions.Like(p.Description??"", pattern) ||
            EF.Functions.Like(p.SKU, pattern)
        ).ToListAsync(ct);
    }
    public async Task<Product?> GetProductBySKUAsync(string SKU,CancellationToken ct = default)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(p => p.SKU == SKU, ct);
    }
}