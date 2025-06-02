namespace JWTSecurity.Services
{
    public interface IAuthService
    {
        bool Verify();
        string GenerateJwtToken(string username);
    }
}
