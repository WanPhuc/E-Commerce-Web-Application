using AuraMart.Identity.Domain.Repositories;

namespace AuraMart.Identity.Repositories.Implements;

public class UserRepository : BaseRepository<User, IdentityDbContext>, IUserRepository
{
    public UserRepository(IdentityDbContext context) : base(context) { }
}
