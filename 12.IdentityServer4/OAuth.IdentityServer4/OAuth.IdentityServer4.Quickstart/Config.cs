using System.Collections.Generic;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace OAuth.IdentityServer4.Quickstart
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
        public static IEnumerable<ApiScope> ApiScopes => new List<ApiScope> { new ApiScope("api1", "My API") };

        /// <summary>
        /// 哪些客户端 Client（应用） 可以使用这个 Authorization Server
        /// </summary>
        public static IEnumerable<Client> Clients => new List<Client>
        {
            new Client
            {
                // 客户端的标识，要是惟一的
                ClientId = "7AAACBC4-3E79-9FB5-F180-03BA7B1FC954",
                // 授权方式，这里采用的是客户端认证模式，只要ClientId，以及ClientSecrets正确即可访问对应的AllowedScopes里面的api资源
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                // 客户端密码，进行了加密
                ClientSecrets =
                {
                    new Secret("8F3697C0-9DA4-666E-14C0-269936AA2692".Sha256())
                },
                // 定义这个客户端可以访问的APi资源数组，上面只有一个api
                AllowedScopes = {"api1"}
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
