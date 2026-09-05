

namespace AuraMart.Identity.Dtos
{
    public class AuthResponseDto
    {
        public MeDto Me { get; set; }=default!;
        public TokenDto Tokens { get; set; }=default!;
    }
}
