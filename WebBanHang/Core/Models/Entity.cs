using System.ComponentModel.DataAnnotations;
namespace WebBanHang.Core.Models;
public class Entity : IEntity
{
    [Key]
    public Guid Id { get; set; }
}