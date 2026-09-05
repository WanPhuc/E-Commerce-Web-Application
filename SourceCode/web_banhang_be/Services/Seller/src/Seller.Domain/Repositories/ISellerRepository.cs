namespace AuraMart.Seller.Domain.Repositories;

public interface ISellerRepository : IBaseRepository<Domain.Seller>
{
    Task<Domain.Seller?> GetByUserIdAsync(Guid userId);
}
