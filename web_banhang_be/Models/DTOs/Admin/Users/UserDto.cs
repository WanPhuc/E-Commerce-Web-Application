using WebBanHang.Models.DTOs.Sellers;

namespace WebBanHang.Models.DTOs.Users;
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
