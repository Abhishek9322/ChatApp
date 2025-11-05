namespace ChatApp.JWTTOEkn.Interface
{
    public interface IJwtTokenService
    {
        string CreateToken(string username);
    }
}
