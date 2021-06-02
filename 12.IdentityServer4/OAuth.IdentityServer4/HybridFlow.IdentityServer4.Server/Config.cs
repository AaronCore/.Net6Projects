using System.Collections.Generic;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace HybridFlow.IdentityServer4.Server
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };


        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("hybrid_scope1")
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {

                new ApiResource("api1","api1")
                {
                    Scopes={ "hybrid_scope1" },
                    UserClaims={JwtClaimTypes.Role},  //添加Cliam 角色类型
                    ApiSecrets={new Secret("apipassword".Sha256())}
                }
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {


                 new Client
                {
                    ClientId = "D521A185-77D6-2959-7487-E3587BBE8D85",
                    ClientName = "hybrid Auth",
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Hybrid,

                    RequirePkce = false,//v4.x需要配置这个

                    RedirectUris ={
                    "http://localhost:5003/signin-oidc", //跳转登录到的客户端的地址
                    },
                    // RedirectUris = {"http://localhost:5003/auth.html" }, //跳转登出到的客户端的地址
                    PostLogoutRedirectUris ={
                        "http://localhost:5003/signout-callback-oidc",
                    },

                    AllowedScopes = {
                           IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                         "hybrid_scope1"
                     },
                      //允许将token通过浏览器传递
                     AllowAccessTokensViaBrowser=true,
                    // AllowOfflineAccess=true,
                     // 是否需要同意授权 （默认是false）
                      RequireConsent=true
                 }
            };
    }
}
