namespace WebBanHang.Models.DTOs.Users;
public class CreateuserDto
{
    public string FullName { get; set; }=default!;
    public string Email { get; set; }=default!;
    public string Password { get; set; }=default!;
}