using System.Collections.Generic;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace ImplicitGrantType.IdentityServer4.Server
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
        public static IEnumerable<ApiScope> ApiScopes => new List<ApiScope> { new ApiScope("implicit_scope1") };

        public static IEnumerable<ApiResource> ApiResources => new List<ApiResource>()
        {
            new ApiResource("api1","api1")
            {
                Scopes = { "implicit_scope1" },
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
                ClientName = "Implicit Auth",
                // 授权方式
                AllowedGrantTypes = GrantTypes.Implicit,
                RedirectUris ={
                    "http://localhost:5002/signin-oidc",  //跳转登录到的客户端的地址
                },
                PostLogoutRedirectUris ={
                    "http://localhost:5002/signout-callback-oidc",//跳转登出到的客户端的地址
                },
                // 定义这个客户端可以访问的APi资源数组，上面只有一个api
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "implicit_scope1"
                },
                // 允许将token通过浏览器传递
                AllowAccessTokensViaBrowser=true,
                // 是否需要同意授权(默认是false)
                RequireConsent = true
            }
        };
    }
}
