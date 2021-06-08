using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.RabbitMQ.Integration
{
    public abstract class RabbitBaseOptions
    {
        /// <summary>
        /// 服务节点，可以是单独的hostname或者IP，也可以是host:port或者ip:port形式
        /// </summary>
        public string[] Hosts { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; } = 5672;
        /// <summary>
        /// 账号
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 虚拟机
        /// </summary>
        public string VirtualHost { get; set; } = "/";
    }
}
