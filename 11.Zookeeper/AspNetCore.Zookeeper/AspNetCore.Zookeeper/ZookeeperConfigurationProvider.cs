using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Zookeeper
{
    public class ZookeeperConfigurationProvider : ConfigurationProvider, IDisposable
    {
        IDisposable _changeTokenRegistration;

        /// <summary>
        /// 监控对象
        /// </summary>
        ZookeeperConfigurationWatcher zookeeperConfigurationWatcher;
        public ZookeeperConfigurationSource ZookeeperConfigurationSource { get; private set; }

        public ZookeeperConfigurationProvider(ZookeeperConfigurationSource zookeeperConfigurationSource)
        {
            this.ZookeeperConfigurationSource = zookeeperConfigurationSource;
            this.zookeeperConfigurationWatcher = new ZookeeperConfigurationWatcher(this);

            if (zookeeperConfigurationSource.ZookeeperOptions.ReloadOnChange)
            {
                _changeTokenRegistration = ChangeToken.OnChange(
                    () => GetReloadToken(),
                    () =>
                    {
                        Thread.Sleep(zookeeperConfigurationSource.ZookeeperOptions.ReloadDelay);
                        Load();
                    });
            }
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        public override void Load()
        {
            Data = zookeeperConfigurationWatcher.Process();
        }
        /// <summary>
        /// 重置
        /// </summary>
        public void Reload()
        {
            OnReload();
        }
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            _changeTokenRegistration?.Dispose();
        }
    }
}
