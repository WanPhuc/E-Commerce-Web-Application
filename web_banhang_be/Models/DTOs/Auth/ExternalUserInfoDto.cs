namespace WebBanHang.Models.DTOs.Auth
{
    public class ExternalUserInfoDto
    {
        public string ProviderId { get; set; }=default!; // ID duy nhất từ Google/FB
        public string Email { get; set; }=default!;
        public string Name { get; set; }=default!;
        public string Provider { get; set; }=default!; // "Google" hoặc "Facebook"
    }
}
