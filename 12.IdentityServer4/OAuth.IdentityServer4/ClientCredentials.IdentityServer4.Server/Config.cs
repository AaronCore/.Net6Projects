using System.Collections.Generic;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace ClientCredentials.IdentityServer4.Server
{
    /// <summary>
    /// IdentityServer4配置
    /// </summary>
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        /// <summary>
        ///  Authorization Server保护了哪些 API Scope（作用域）
        /// </summary>
        public static IEnumerable<ApiScope> ApiScopes => new List<ApiScope> { new ApiScope("client_scope1") };

        public static IEnumerable<ApiResource> ApiResources => new List<ApiResource>()
        {
            new ApiResource("api1","api1")
            {
                Scopes = { "client_scope1" }
            }
        };

        /// <summary>
        /// 哪些客户端 Client（应用） 可以使用这个 Authorization Server
        /// </summary>
        public static IEnumerable<Client> Clients => new List<Client>
        {
            new Client
            {
                // 客户端的标识，要是唯一的
                ClientId = "D521A185-77D6-2959-7487-E3587BBE8D85",
                ClientName = "Client Credentials Client",
                // 授权方式，这里采用的是客户端认证模式，只要ClientId，以及ClientSecrets正确即可访问对应的AllowedScopes里面的api资源
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                // 客户端密码，进行了加密
                ClientSecrets =
                {
                    new Secret("6CA372B2-54F8-CF26-B4DB-2E28BBF5E9B8".Sha256())
                },
                // 定义这个客户端可以访问的APi资源数组，上面只有一个api
                AllowedScopes = { "client_scope1" }
            }
        };

        /// <summary>
        ///  哪些User可以被这个Authorization Server识别并授权
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<TestUser> Users()
        {
            return new List<TestUser>()
            {
                new TestUser()
                {
                    SubjectId="001",
                    Username="test001",
                    Password="123123"
                }
            };
        }
    }
}
