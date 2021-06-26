using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Nacos.V2;

namespace NacosApp2.Controllers
{
    public class ConfigController : ControllerBase
    {
        private readonly INacosConfigService _svc;
        public ConfigController(INacosConfigService svc)
        {
            _svc = svc;
        }

        [HttpGet]
        public async Task<string> GetConfig(string d)
        {
            var res = await _svc.GetConfig(d, Nacos.V2.Common.Constants.DEFAULT_GROUP, 3000).ConfigureAwait(false);
            return res;
        }

        [HttpGet]
        public async Task<bool> Delete(string d)
        {
            var res = await _svc.RemoveConfig(d, Nacos.V2.Common.Constants.DEFAULT_GROUP).ConfigureAwait(false);
            return res;
        }

        [HttpGet]
        public async Task<bool> Publish(string d)
        {
            var content = new System.Random().Next(1, 9999999).ToString();
            var res = await _svc.PublishConfig(d, Nacos.V2.Common.Constants.DEFAULT_GROUP, content).ConfigureAwait(false);
            return res;
        }

        [HttpGet]
        public async Task<string> Listen(string d)
        {
            await _svc.AddListener(d, Nacos.V2.Common.Constants.DEFAULT_GROUP, ConfigListen).ConfigureAwait(false);
            return "al ok";
        }

        [HttpGet]
        public async Task<string> UnListen(string d)
        {
            await _svc.RemoveListener(d, Nacos.V2.Common.Constants.DEFAULT_GROUP, ConfigListen).ConfigureAwait(false);
            return "rl ok";
        }

        private static readonly CusConfigListen ConfigListen = new CusConfigListen();
        public class CusConfigListen : Nacos.V2.IListener
        {
            public void ReceiveConfigInfo(string configInfo)
            {
                System.Console.WriteLine("config cb cb cb " + configInfo);
            }
        }
    }
}
