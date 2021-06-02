using System.Collections.Generic;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace AuthorizationCode.IdentityServer4.Server
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
        public static IEnumerable<ApiScope> ApiScopes => new List<ApiScope> { new ApiScope("code_scope1") };

        public static IEnumerable<ApiResource> ApiResources => new List<ApiResource>()
        {
            new ApiResource("api1","api1")
            {
                Scopes = { "code_scope1" },
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
                ClientName = "Authorization Code",
                // 授权方式，这里采用的是客户端认证模式，只要ClientId，以及ClientSecrets正确即可访问对应的AllowedScopes里面的api资源
                AllowedGrantTypes = GrantTypes.Code,
                RedirectUris ={
                    "http://localhost:5003/signin-oidc", //跳转登录到的客户端的地址
                },
                // RedirectUris = {"http://localhost:5003/auth.html" }, //跳转登出到的客户端的地址
                PostLogoutRedirectUris ={
                    "http://localhost:5003/signout-callback-oidc",
                },
                ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },
                AllowedScopes = {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "code_scope1"
                },
                AlwaysIncludeUserClaimsInIdToken=true,
                //允许将token通过浏览器传递
                AllowAccessTokensViaBrowser=true,
                // 是否需要同意授权 （默认是false）
                RequireConsent=true
            }
        };
    }
}
