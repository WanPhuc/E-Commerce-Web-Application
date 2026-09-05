namespace AuraMart.Identity.Contracts;

public record UserSnapshot(
    Guid Id,
    string FullName,
    string Email,
    bool IsActive);
