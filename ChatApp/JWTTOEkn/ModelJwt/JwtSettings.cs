namespace ChatApp.JWTTOEkn.ModelJwt
{
    public class JwtSettings
    {
        public string Key { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public int ExpMinutes { get; set; } = 60;
    }
}
