namespace AuraMart.Cart.Domain.Repositories;

public interface ICartRepository : IBaseRepository<Domain.Cart>
{
    Task<Domain.Cart?> GetByUserIdAsync(Guid userId);
}
