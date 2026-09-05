namespace AuraMart.Catalog.Application;

public class SellerRef
{
    public Guid Id { get; init; }
    public string StoreName { get; init; } = default!;
}

public interface ISellerLookup
{
    Task<SellerRef?> FindByUserIdAsync(Guid userId);
}
