

namespace AuraMart.Catalog.Repositories.Implements;

public class CategoryRepository : BaseRepository<Category, CatalogDbContext>, ICategoryRepository
{
    public CategoryRepository(CatalogDbContext context) : base(context) { }
}
