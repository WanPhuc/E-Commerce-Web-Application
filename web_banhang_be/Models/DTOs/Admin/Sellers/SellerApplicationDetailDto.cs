using WebBanHang.Models.Enums;

namespace WebBanHang.Models.DTOs.Sellers;
public class SellerApplicationDetailDto
{
    public Guid Id { get; set; }
    public string StoreName { get; set; }=default!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public SellerApplicationStatus Status { get; set; }=default!;

    public Guid UserId { get; set; }
    public string FullName { get; set; }=default!;
    public string Email { get; set; }=default!;
    public DateTime? ReviewedAt { get; set; }

}