using JWTSecurity.DTO;
using JWTSecurity.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTSecurity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class authController : ControllerBase
    {
        private readonly IAuthService _authService;
        public authController(IAuthService authService)
        {
            _authService = authService; 
        }
        [HttpPost("login")]
        public IActionResult GetToken(UserLogin user)
        {
            var token = _authService.GenerateJwtToken(user.username);
            return Ok(new {token=token});
        }
        [HttpPost("logout")]
        public IActionResult Logout()
        {
           
            //create token
            _authService.RevokeToken();
            return Ok();
        }
        [HttpPost("changePassword")]
        public IActionResult ChangePassword()
        {

            //create token
            _authService.ChangePassword();
            return Ok();
        }
    }
}
