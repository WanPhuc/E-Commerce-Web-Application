using AuraMart.Identity.Domain.Repositories;

namespace AuraMart.Identity.Repositories.Implements;

public class RoleRepository : BaseRepository<Role, IdentityDbContext>, IRoleRepository
{
    public RoleRepository(IdentityDbContext context) : base(context) { }
}
