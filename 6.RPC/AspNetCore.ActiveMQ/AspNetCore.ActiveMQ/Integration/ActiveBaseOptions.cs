namespace AspNetCore.ActiveMQ.Integration
{
    public class ActiveBaseOptions
    {
        /// <summary>
        /// 是否集群模式
        /// </summary>
        public bool IsCluster { get; set; }
        /// <summary>
        /// 服务节点，可以是单独的hostname或者IP，也可以是host:port或者ip:port形式，也可以是host:port
        /// </summary>
        public string[] BrokerUris { get; set; }
        /// <summary>
        /// 设置
        /// </summary>
        public string Query { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }
}
