using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Kafka.Integration
{
    public abstract class KafkaBaseOptions
    {
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string[] BootstrapServers { get; set; }
        /// <summary>
        /// SASL PLAINTEXT认证用户名
        /// </summary>
        public string SaslUsername { get; set; }
        /// <summary>
        /// SASL PLAINTEXT认证密码
        /// </summary>
        public string SaslPassword { get; set; }
    }
}
