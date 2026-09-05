namespace AuraMart.Identity.Dtos{
    public class RefreshTokenRequestDto
    {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; }=default!;
    }
}
