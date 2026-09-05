

namespace AuraMart.Catalog.Repositories.Implements;

public class ProductReviewRepository : BaseRepository<ProductReview, CatalogDbContext>, IProductReviewRepository
{
    public ProductReviewRepository(CatalogDbContext context) : base(context) { }
}
