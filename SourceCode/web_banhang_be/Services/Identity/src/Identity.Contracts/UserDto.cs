namespace AuraMart.Identity.Dtos;
public class UserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }=default!;
    public string Email { get; set; }=default!;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public string Role { get; set; }=default!;
    public SellerSummaryDto? Seller { get; set; }


}

public class SellerSummaryDto
{
    public Guid Id { get; set; }
    public string ShopName { get; set; } = default!;
    public SellerApplicationStatus Status { get; set; }
}
