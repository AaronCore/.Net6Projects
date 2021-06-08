using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Zookeeper
{
    public class ZookeeperOptions
    {
        /// <summary>
        /// 是否监控源数据变化
        /// </summary>
        public bool ReloadOnChange { get; set; } = true;
        /// <summary>
        /// 加载延迟
        /// </summary>
        public int ReloadDelay { get; set; } = 250;
        /// <summary>
        /// Zookeeper集群地址
        /// </summary>
        public string[] Address { get; set; }
        /// <summary>
        /// Zookeeper初始路径
        /// </summary>
        public string RootPath { get; set; }
        /// <summary>
        /// 认证主题
        /// </summary>
        public AuthScheme Scheme { get; set; }
        /// <summary>
        /// 认证信息
        /// </summary>
        public string Auth { get; set; }
        /// <summary>
        /// 生成的配置中是否去掉初始路径
        /// </summary>
        public bool ExcludeRoot { get; set; }
    }
}
