namespace AuraMart.Seller.Domain.Repositories;

public interface ISellerApplicationRepository : IBaseRepository<SellerApplication>
{
    Task<SellerApplication?> GetByUserIdAsync(Guid userId);
    Task<List<SellerApplication>> GetPendingApplicationsAsync();
}
