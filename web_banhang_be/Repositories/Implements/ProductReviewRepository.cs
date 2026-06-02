using Microsoft.EntityFrameworkCore;
using WebBanHang.Data;
using WebBanHang.Models.EntityModels;
using WebBanHang.Repositories.Interfaces;

namespace WebBanHang.Repositories.SqlServer;
public class ProductReviewRepository : BaseRepository<ProductReview>, IProductReviewRepository
{
    public ProductReviewRepository(AppDbContext context) : base(context)
    {
    }

    
}