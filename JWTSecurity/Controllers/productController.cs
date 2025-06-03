using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Text.Json;

namespace JWTSecurity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class productController : ControllerBase
    {
        private readonly IHttpContextAccessor _context;
        public productController(IHttpContextAccessor context)
        {
            _context = context;
        }
        [HttpGet,Authorize]
        public IActionResult getProduct()
        {        
            try
            {
                var user = _context.HttpContext.User;
                if (user.Identity.IsAuthenticated)
                {
                    //JsonSerializer.Deserialize<type>()
                    return Ok(new 
                    {
                        uid = user.FindFirst("uid")?.Value!,
                        jit = user.FindFirst("jti")?.Value!,
                        iat = user.FindFirst("iat")?.Value!,
                        exp = user.FindFirst("exp")?.Value!,                  
                    });
                }
                else
                {
                    return BadRequest();
                }            
            }
            catch
            {
                return BadRequest();               
            }
        }
    }
}
