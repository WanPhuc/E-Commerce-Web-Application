using System.ComponentModel.DataAnnotations;

namespace VanFucVN.Core.Persistence.Domain;

public abstract class Entity : IEntity
{
    [Key]
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool DeleteFlg { get; set; }
}
