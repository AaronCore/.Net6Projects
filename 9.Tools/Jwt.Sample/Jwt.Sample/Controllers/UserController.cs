using Jwt.Sample.Common;
using Jwt.Sample.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Jwt.Sample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        public UserController(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost]
        [Route("get_token")]
        public IActionResult GetToken()
        {
            return Ok(JwtHelper.Token(_jwtSettings));
        }

        [Authorize]
        [HttpPost]
        [Route("get_user")]
        public IActionResult GetUser()
        {
            //获取当前请求用户的信息，包含token信息
            var user = HttpContext.User;
            return Ok();
        }
    }
}
