using JWTSecurity.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTSecurity.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;   
        private readonly IDistributedCache _distributedCache;
        private readonly IHttpContextAccessor _context;
        public AuthService(
            IConfiguration configuration,     
            IDistributedCache distributedCache,
            IHttpContextAccessor context)
        {
            _configuration = configuration;   
            _distributedCache = distributedCache;
            _context = context;
        }

        public void ChangePassword()
        {
            var decode = _context.HttpContext.User;         
            _distributedCache.SetString($"token_iat_enable_{decode.FindFirst("uid")?.Value}", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
        }

        public string GenerateJwtToken(string username)
        {
            var claims = new[]
            {
               new Claim("uid",username),
               new Claim(JwtRegisteredClaimNames.Sub,username),
               new Claim(JwtRegisteredClaimNames.Iat,DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
               new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Hostname"],
                audience: _configuration["Hostname"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: cred
                );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }

        public void RevokeToken()
        {         
            var token = _context.HttpContext!.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var decode = handler.ReadJwtToken(token);          
            _distributedCache.SetString($"token_black_list_{decode.Payload["uid"]}_{decode.Payload.Jti}", token);          
        }

        
    }
}
