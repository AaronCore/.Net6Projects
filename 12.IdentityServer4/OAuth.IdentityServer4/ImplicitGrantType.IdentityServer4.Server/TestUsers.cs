using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Test;


namespace ImplicitGrantType.IdentityServer4.Server
{
    public class TestUsers
    {
        public static List<TestUser> Users
        {
            get
            {
                var address = new
                {
                    street_address = "One Hacker Way",
                    locality = "Heidelberg",
                    postal_code = 69118,
                    country = "Germany"
                };

                return new List<TestUser>
                {
                    new TestUser
                    {
                        SubjectId = "100",
                        Username = "aaron",
                        Password = "123456",
                        Claims =
                        {
                            new Claim(JwtClaimTypes.Name, "wuHui"),
                            new Claim(JwtClaimTypes.GivenName, "hui"),
                            new Claim(JwtClaimTypes.FamilyName, "wu"),
                            new Claim(JwtClaimTypes.Email, "aaron@email.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.WebSite, "http://aaron.com"),
                            new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address), IdentityServerConstants.ClaimValueTypes.Json),
                            new Claim(JwtClaimTypes.Role,"admin")  //添加角色
                        },
                    }
                };
            }
        }
    }
}
