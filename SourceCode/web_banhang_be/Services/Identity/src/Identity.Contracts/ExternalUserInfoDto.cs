namespace AuraMart.Identity.Dtos
{
    public class ExternalUserInfoDto
    {
        public string ProviderId { get; set; }=default!; // ID duy nh?t t? Google/FB
        public string Email { get; set; }=default!;
        public string Name { get; set; }=default!;
        public string Provider { get; set; }=default!; // "Google" ho?c "Facebook"
    }
}
