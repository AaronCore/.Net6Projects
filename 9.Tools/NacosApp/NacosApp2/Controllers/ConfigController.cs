using Microsoft.AspNetCore.Mvc;
using Nacos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nacos.V2;

namespace NacosApp2.Controllers
{
    public class ConfigController : Controller
    {
        private readonly INacosConfigService _svc;
        public ConfigController(INacosConfigService svc)
        {
            _svc = svc;
        }

        [HttpGet]
        public async Task<IActionResult> GetConfig(string text)
        {
            var dataId = "pub-1";
            var res = await _svc.GetConfig(dataId, Nacos.V2.Common.Constants.DEFAULT_GROUP, 3000).ConfigureAwait(false);
            return Json(res);
        }

        [HttpGet]
        public async Task<IActionResult> Publish()
        {
            var dataId = "pub-1";
            var content = "test-value";
            var res = await _svc.PublishConfig(dataId, Nacos.V2.Common.Constants.DEFAULT_GROUP, content, "json").ConfigureAwait(false);
            return Json(res);
        }
    }
}
