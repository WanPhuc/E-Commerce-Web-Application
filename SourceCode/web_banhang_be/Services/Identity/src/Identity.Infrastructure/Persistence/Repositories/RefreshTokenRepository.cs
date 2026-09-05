using AuraMart.Identity.Domain.Repositories;

namespace AuraMart.Identity.Repositories.Implements;

public class RefreshTokenRepository : BaseRepository<RefreshToken, IdentityDbContext>, IRefreshTokenRepository
{
    public RefreshTokenRepository(IdentityDbContext context) : base(context) { }
}
