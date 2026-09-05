namespace AuraMart.Identity.Contracts;

public interface IUserReader
{
    Task<UserSnapshot?> GetUserAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<UserSnapshot>> GetUsersByIdsAsync(IReadOnlyList<Guid> userIds, CancellationToken ct = default);
    Task<int> CountUsersAsync(CancellationToken ct = default);

    // Phase 5: thong ke cho admin read model
    Task<int> CountRegisteredBeforeAsync(DateTime utcExclusive, CancellationToken ct = default);
    Task<IReadOnlyList<DateTime>> GetRegistrationDatesAsync(DateTime fromUtc, DateTime toUtcExclusive, CancellationToken ct = default);
}
