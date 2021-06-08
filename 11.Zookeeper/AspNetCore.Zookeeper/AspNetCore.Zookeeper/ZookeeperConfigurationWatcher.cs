using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Zookeeper
{
    public class ZookeeperConfigurationWatcher : IDisposable
    {
        ZookeeperConfigurationProvider zookeeperConfigurationProvider;

        /// <summary>
        /// Zookeeper辅助操作类
        /// </summary>
        ZookeeperHelper zookeeperHelper;

        public ZookeeperConfigurationWatcher(ZookeeperConfigurationProvider zookeeperConfigurationProvider)
        {
            this.zookeeperConfigurationProvider = zookeeperConfigurationProvider;
            var options = zookeeperConfigurationProvider.ZookeeperConfigurationSource.ZookeeperOptions;
            zookeeperHelper = new ZookeeperHelper(options.Address, options.RootPath);
            //初次连接后，立即
            zookeeperHelper.OnConnected += () =>
            {
                if (options.Scheme != AuthScheme.World)
                {
                    zookeeperHelper.Authorize(options.Scheme, options.Auth);
                }
                zookeeperConfigurationProvider.Reload();
            };
            zookeeperHelper.Connect();
            //int timeOut = 10;
            //while (!zookeeperHelper.Connected && timeOut-- > 0)
            //{
            //    Thread.Sleep(1000); //停一秒，等待连接完成
            //}
            if (!zookeeperHelper.Connected)
            {
                throw new TimeoutException($"connect to zookeeper [{options.Address}] timeout");
            }

            if (options.ReloadOnChange)
            {
                //监听根节点下所有的znode节点，当任意节点发生改变后，即立刻重新加载
                zookeeperHelper.WatchAllAsync(ze =>
                {
                    if (ze.Type != ZookeeperEvent.EventType.None)
                    {
                        //使用一个异步去完成，同步会造成死锁
                        Task.Run(() =>
                        {
                            zookeeperConfigurationProvider.Reload();
                        });
                    }
                }).Wait();
            }
        }

        /// <summary>
        /// 获取Zookeeper中的数据
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, string> Process()
        {
            var data = zookeeperHelper.GetDictionaryAsync(ConfigurationPath.KeyDelimiter).GetAwaiter().GetResult();
            if (!zookeeperConfigurationProvider.ZookeeperConfigurationSource.ZookeeperOptions.ExcludeRoot)
            {
                return data;
            }

            var root = this.zookeeperConfigurationProvider.ZookeeperConfigurationSource.ZookeeperOptions.RootPath ?? "";
            var rootSplits = root.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            IDictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var key in data.Keys)
            {
                var split = key.Split(new string[] { ConfigurationPath.KeyDelimiter }, StringSplitOptions.RemoveEmptyEntries);
                var array = split.Skip(rootSplits.Length).ToArray();
                dict[ConfigurationPath.Combine(array)] = data[key];
            }
            return dict;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose()
        {
            zookeeperHelper.Dispose();
            zookeeperConfigurationProvider.Dispose();
        }
    }
}
