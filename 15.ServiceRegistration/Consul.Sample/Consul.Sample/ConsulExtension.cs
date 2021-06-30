using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Consul.Sample
{
    public static class ConsulExtension
    {
        public static void ConsulRegister(this IConfiguration configuration)
        {
            // https://www.cnblogs.com/zoe-zyq/p/14580552.html
            var configConsul = configuration.GetSection("Consul");
            var client = new ConsulClient(setup =>
            {
                setup.Address = new Uri(configConsul["ConsulAddr"]);
                setup.Datacenter = "dc1";
            });

            string serviceIp = configConsul["ServiceIP"];
            string servicePort = configConsul["ServicePort"];
            string serviceTags = configConsul["Tags"];
            client.Agent.ServiceRegister(new AgentServiceRegistration()
            {
                ID = "Code6688",
                Name = "Code6688Name",
                Address = serviceIp,
                Port = Convert.ToInt32(servicePort),
                Tags = serviceTags.Split(","),
                Check = new AgentServiceCheck()
                {
                    HTTP = "http://127.0.0.1:5000/weatherforecast",
                    Interval = TimeSpan.FromSeconds(5),
                    Timeout = TimeSpan.FromSeconds(10),
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(20)
                }
            });
        }
    }
}
