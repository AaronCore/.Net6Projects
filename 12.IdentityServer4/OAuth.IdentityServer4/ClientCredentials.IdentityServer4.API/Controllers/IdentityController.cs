using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
