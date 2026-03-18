using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebBanHang.Core.Models;
namespace WebBanHang.Models;
public class Role : Entity
{
    [Required]
    [MaxLength(100)]
    public required string Name {get; set;}

    [Required]
    [MaxLength(500)]
    public required string Description {get; set;}

    public UserPermission Permissions {get; set;} = UserPermission.None;

    [JsonIgnore]
    public IEnumerable<User> Users {get; set;} = new List<User>();
}