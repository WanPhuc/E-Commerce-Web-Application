using Microsoft.EntityFrameworkCore;

namespace AuraMart.Seller.Infrastructure.Persistence.Repositories;

public class SellerRepository : BaseRepository<Domain.Seller, SellerDbContext>, ISellerRepository
{
    public SellerRepository(SellerDbContext context) : base(context) { }

    public async Task<Domain.Seller?> GetByUserIdAsync(Guid userId)
    {
        return await _db.Sellers
            .FirstOrDefaultAsync(s => s.UserId == userId && !s.DeleteFlg);
    }
}
