using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AuthorizationCode.IdentityServer4.Mvc.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json.Linq;

namespace AuthorizationCode.IdentityServer4.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Privacy()
        {
            ViewData["Message"] = "Secure page.";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
            return SignOut("Cookies", "oidc");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// 测试请求API资源(api1)
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> getApi()
        {
            var client = new HttpClient();
            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            if (string.IsNullOrEmpty(accessToken))
            {
                return Json(new { msg = "accesstoken 获取失败" });
            }
            // client.SetBearerToken(accesstoken);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            //var httpResponse = await client.GetAsync("http://localhost:5002/Identity/GetUserClaims");
            //var result = await httpResponse.Content.ReadAsStringAsync();
            //if (!httpResponse.IsSuccessStatusCode)
            //{
            //    return Json(new { msg = "请求 api1 失败。", error = result });
            //}
            //return Json(new
            //{
            //    msg = "成功",
            //    data = JsonConvert.DeserializeObject(result)
            //});
            var content = await client.GetStringAsync("http://localhost:5002/Identity/GetUserClaims");

            ViewBag.Json = JArray.Parse(content).ToString();
            return View("json");
        }
    }
}
