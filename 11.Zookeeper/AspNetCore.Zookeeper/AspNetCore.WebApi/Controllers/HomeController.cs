using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        /// <summary>
        /// 输出
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public TestOptions Get()
        {
            //这里不要使用IOptionsMonitor<>，IOption<>等，因为IOptions<>不能改变数据，而IOptionsMonitor<>有缓存IOptionsMonitorCache<>
            //推荐使用IOptionsSnapshot<>
            var options = HttpContext.RequestServices.GetService<IOptionsSnapshot<TestOptions>>();
            return options.Get("");
        }
    }
}
