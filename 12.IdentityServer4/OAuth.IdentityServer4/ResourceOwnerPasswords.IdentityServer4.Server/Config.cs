using System.Collections.Generic;
using IdentityModel;
using IdentityServer4.Models;

namespace ResourceOwnerPasswords.IdentityServer4.Server
{
    /// <summary>
    /// IdentityServer4配置
    /// </summary>
    public class Config
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
        public static IEnumerable<ApiScope> ApiScopes => new List<ApiScope> { new ApiScope("password_scope1") };

        public static IEnumerable<ApiResource> ApiResources => new List<ApiResource>()
        {
            new ApiResource("api1","api1")
            {
                Scopes = { "password_scope1" },
                UserClaims={JwtClaimTypes.Role},
                ApiSecrets = {new Secret("apipassword".Sha256())}
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
                ClientName = "Resource Owner Password",
                // 授权方式
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                // 客户端密码，进行了加密
                ClientSecrets =
                {
                    new Secret("6CA372B2-54F8-CF26-B4DB-2E28BBF5E9B8".Sha256())
                },
                // 定义这个客户端可以访问的APi资源数组，上面只有一个api
                AllowedScopes = { "password_scope1" }
            }
        };
    }
}
