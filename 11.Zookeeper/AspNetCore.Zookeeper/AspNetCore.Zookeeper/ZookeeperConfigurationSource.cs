using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Zookeeper
{
    public class ZookeeperConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// 源状态信息
        /// </summary>
        public ZookeeperOptions ZookeeperOptions { get; private set; }

        public ZookeeperConfigurationSource(ZookeeperOptions zookeeperOptions)
        {
            this.ZookeeperOptions = zookeeperOptions;
        }

        /// <summary>
        /// 获取配置提供者
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ZookeeperConfigurationProvider(this);
        }
    }
}
