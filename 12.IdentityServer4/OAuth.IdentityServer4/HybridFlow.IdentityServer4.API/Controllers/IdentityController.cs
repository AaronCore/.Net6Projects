using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HybridFlow.IdentityServer4.API.Controllers
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
