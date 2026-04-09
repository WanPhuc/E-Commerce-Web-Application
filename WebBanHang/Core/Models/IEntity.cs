namespace WebBanHang.Core.Models;
public interface IEntity
{
    Guid Id{ get; set; }
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
    bool DeleteFlg { get; set; }
}