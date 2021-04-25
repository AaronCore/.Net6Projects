using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IdentityModel;
using Jwt.Sample.Model;
using Microsoft.IdentityModel.Tokens;

namespace Jwt.Sample.Common
{
    public class JwtHelper
    {
        public static object Token(JwtSettings jwtSettings)
        {
            //测试自己创建的对象
            var user = new User
            {
                Id = 1,
                Name = "001",
                Password = "e10adc3949ba59abbe56e057f20f883e"
            };
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
            var authTime = DateTime.Now;//授权时间
            var expiresAt = authTime.AddDays(30);//过期时间
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(JwtClaimTypes.Audience,jwtSettings.Audience),
                    new Claim(JwtClaimTypes.Issuer,jwtSettings.Issuer),
                    new Claim(JwtClaimTypes.Id, user.Id.ToString()),
                    new Claim(JwtClaimTypes.Name, user.Name)
                }),
                Expires = expiresAt,
                //对称秘钥SymmetricSecurityKey
                //签名证书(秘钥，加密算法)SecurityAlgorithms
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            var result = new
            {
                access_token = tokenString,
                token_type = "Bearer",
                profile = new
                {
                    id = user.Id,
                    user = user.Name,
                    auth_time = authTime,
                    expires_at = expiresAt
                }
            };
            return result;
        }
    }
}
