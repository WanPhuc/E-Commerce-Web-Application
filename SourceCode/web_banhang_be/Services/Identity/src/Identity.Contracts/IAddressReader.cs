namespace AuraMart.Identity.Contracts;

public interface IAddressReader
{
    Task<AddressSnapshot?> GetAddressAsync(Guid addressId, CancellationToken ct = default);
}

public record AddressSnapshot(
    Guid Id,
    Guid UserId,
    string RecipientName,
    string PhoneNumber,
    string? AddressLine,
    string Ward,
    string District,
    string City,
    bool IsDefault);
