using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace ClientCredentials.IdentityServer4.API.Controllers
{
    [Route("Identity")]
    [Authorize]
    public class IdentityController : Controller
    {
        [HttpGet("GetUserClaims")]
        public IActionResult GetUserClaims()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }
    }
}
