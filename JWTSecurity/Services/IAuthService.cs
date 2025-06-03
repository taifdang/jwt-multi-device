using JWTSecurity.Models;

namespace JWTSecurity.Services
{
    public interface IAuthService
    {      
        string GenerateJwtToken(string username);
        void RevokeToken();
        void ChangePassword();
    }
}
