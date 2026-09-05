

namespace AuraMart.Catalog.Repositories.Implements;

public class ProductRepository : BaseRepository<Product, CatalogDbContext>, IProductRepository
{
    public ProductRepository(CatalogDbContext context) : base(context) { }
}
