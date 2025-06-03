using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace JWTSecurity.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class CheckTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public CheckTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext,IDistributedCache connection)
        {
            //check request
            var token = httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
           
            if (!string.IsNullOrEmpty(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var decode = handler.ReadJwtToken(token);
                var blacklist = connection.GetString($"token_black_list_{decode.Payload["uid"]}_{decode.Payload.Jti}");
                if (!string.IsNullOrEmpty(blacklist))
                {
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await httpContext.Response.WriteAsync("token revoked");
                    return;
                }

                var changepasstime = connection.GetString($"token_iat_enable_{decode.Payload["uid"]}");
                var longIss = new DateTimeOffset(decode.Payload.IssuedAt).ToUnixTimeSeconds();
                if (!string.IsNullOrEmpty(changepasstime) && longIss < Convert.ToInt64(Convert.ToDecimal(changepasstime)))
                {
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await httpContext.Response.WriteAsync("token revoked: [event] change password");
                    return;
                }
                //set decode token ,JsonSerializer.Serialize(x.Value) 
                var claims = decode.Payload.Select(x =>new Claim(x.Key, x.Value.ToString())).ToList();
                var identity = new ClaimsIdentity(claims, "jwt");
                var principal = new ClaimsPrincipal(identity);
                httpContext.User = principal;
            }
            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CheckTokenMiddlewareExtensions
    {
        public static IApplicationBuilder UseCheckTokenMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CheckTokenMiddleware>();
        }
    }
}
