namespace AuraMart.Identity.Contracts;

// Write-contract cho module khac cap nhat thong tin profile/address
// (bridge Phase 2 - giu hanh vi cu, Phase 3 co the thay bang command/event)
public interface IProfileWriter
{
    Task<bool> UpdateUserProfileAsync(Guid userId, string fullName, string email, CancellationToken ct = default);
    Task<bool> UpdateAddressAsync(Guid addressId, AddressSnapshot data, CancellationToken ct = default);
    Task<Guid> CreateAddressAsync(Guid userId, AddressSnapshot data, CancellationToken ct = default);
}
