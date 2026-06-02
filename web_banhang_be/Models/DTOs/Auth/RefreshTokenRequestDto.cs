namespace Blog.Models.Dto.Auth
{
    public class RefreshTokenRequestDto
    {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; }=default!;
    }
}
