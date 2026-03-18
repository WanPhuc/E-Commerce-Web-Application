using WebBanHang.Models.DTOs.Sellers;

namespace WebBanHang.Services.Interfaces;

public interface ISellerApplicationService
{
    Task <SellerApplicationDetailDto> GetSellerApplicationDetailByIdAsync(Guid applicationId);
    Task ApproveSellerApplicationAsync(Guid applicationId);
    Task RejectSellerApplicationAsync(Guid applicationId);
}