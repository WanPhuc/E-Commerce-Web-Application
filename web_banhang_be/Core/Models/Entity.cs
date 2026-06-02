using System.ComponentModel.DataAnnotations;
namespace WebBanHang.Core.Models;
public class Entity : IEntity
{
    [Key]
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool DeleteFlg { get; set; }
}