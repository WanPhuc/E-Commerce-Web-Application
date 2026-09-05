using System.ComponentModel.DataAnnotations;

namespace AuraMart.Identity.Domain;
public class User : Entity
{
    public byte ProviderType { get; set; }
    public string? ProviderUserId { get; set; }

    [Required(ErrorMessage = "Họ và tên là bắt buộc.")]
    [MaxLength(50, ErrorMessage = "Họ và tên không được vượt quá 50 ký tự.")]
    [Display(Name = "Họ và tên")]
    public required string FullName { get; set; }

    [Required(ErrorMessage = "Email là bắt buộc.")]
    [MaxLength(50, ErrorMessage = "Email không được vượt quá 50 ký tự.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [Display(Name = "Email")]
    public required string Email { get; set; }

    public string? PasswordHash { get; set; }

    public bool IsActive { get; set; } = true;

    [Required]
    public required Guid RoleId { get; set; }
    public Role Role { get; set; }= default!;

    public virtual ICollection<Address> Addresses { get; set; }= new List<Address>();
}
