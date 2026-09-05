using Microsoft.EntityFrameworkCore;

namespace AuraMart.Cart.Infrastructure.Persistence.Repositories;

public class CartRepository : BaseRepository<Domain.Cart, CartDbContext>, ICartRepository
{
    public CartRepository(CartDbContext context) : base(context) { }

    public async Task<Domain.Cart?> GetByUserIdAsync(Guid userId)
    {
        return await _db.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.DeleteFlg);
    }
}
