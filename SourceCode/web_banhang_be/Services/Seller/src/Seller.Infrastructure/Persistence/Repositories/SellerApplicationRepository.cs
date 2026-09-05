using Microsoft.EntityFrameworkCore;

namespace AuraMart.Seller.Infrastructure.Persistence.Repositories;

public class SellerApplicationRepository : BaseRepository<SellerApplication, SellerDbContext>, ISellerApplicationRepository
{
    public SellerApplicationRepository(SellerDbContext context) : base(context) { }

    public async Task<SellerApplication?> GetByUserIdAsync(Guid userId)
    {
        return await _db.SellerApplications
            .OrderByDescending(a => a.CreatedAt)
            .FirstOrDefaultAsync(a => a.UserId == userId && !a.DeleteFlg);
    }

    public async Task<List<SellerApplication>> GetPendingApplicationsAsync()
    {
        return await _db.SellerApplications
            .Where(a => a.Status == "Pending" && !a.DeleteFlg)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }
}
