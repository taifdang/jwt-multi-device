using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTSecurity.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateJwtToken(string username)
        {
            throw new NotImplementedException();
        }

        public bool Verify()
        {
            throw new NotImplementedException();
        }
    }
}
