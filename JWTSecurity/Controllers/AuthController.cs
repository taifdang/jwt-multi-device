using JWTSecurity.DTO;
using JWTSecurity.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTSecurity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService; 
        }
        [HttpPost("login")]
        public IActionResult GetToken(UserLogin user)
        {

            var token = _authService.GenerateJwtToken(user.username);
            return Ok(new {token=token});
        }
    }
}
