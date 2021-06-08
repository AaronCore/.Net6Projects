using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Zookeeper
{
    public static class ZookeeperConfigurationExtensions
    {
        /// <summary>
        /// 添加Zookeeper配置
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configure"></param>
        public static IConfigurationBuilder AddZookeeper(this IConfigurationBuilder builder, Action<ZookeeperOptions> configure)
        {
            var options = new ZookeeperOptions();
            configure?.Invoke(options);
            return builder.AddZookeeper(options);
        }
        /// <summary>
        /// 添加Zookeeper配置
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="zookeeperOptions"></param>
        public static IConfigurationBuilder AddZookeeper(this IConfigurationBuilder builder, ZookeeperOptions zookeeperOptions)
        {
            ZookeeperConfigurationSource zookeeperConfigurationSource = new ZookeeperConfigurationSource(zookeeperOptions);
            builder.Add(zookeeperConfigurationSource);
            return builder;
        }
        /// <summary>
        /// 添加Zookeeper配置
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IHostBuilder UseZookeeper(this IHostBuilder builder, Action<ZookeeperOptions> configure)
        {
            var options = new ZookeeperOptions();
            configure?.Invoke(options);
            return builder.UseZookeeper(options);
        }
        /// <summary>
        /// 添加Zookeeper配置
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="zookeeperOptions"></param>
        /// <returns></returns>
        public static IHostBuilder UseZookeeper(this IHostBuilder builder, ZookeeperOptions zookeeperOptions)
        {
            return builder.ConfigureAppConfiguration((_, cbuilder) => cbuilder.AddZookeeper(zookeeperOptions));
        }
    }
}
