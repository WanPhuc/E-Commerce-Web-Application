using Blog.Models.Dto.Auth;

namespace WebBanHang.Models.DTOs.Auth
{
    public class AuthResponseDto
    {
        public MeDto Me { get; set; }=default!;
        public TokenDto Tokens { get; set; }=default!;
    }
}
