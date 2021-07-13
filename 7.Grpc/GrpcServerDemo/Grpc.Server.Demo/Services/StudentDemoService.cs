using Grpc.Core;
using Grpc.Server.Demo.Permission;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Server.Demo.Services
{
    //[Authorize]
    [Authorize(Policy = "Permission")]
    public class StudentDemoService: StudentService.StudentServiceBase
    {
        private PermissionRequirement _permissionRequirement;

        public StudentDemoService(PermissionRequirement permissionRequirement)
        {
            _permissionRequirement = permissionRequirement;
        }

        public override Task<StudentResponse> GetStudentByUserName(QueryStudentRequest request, ServerCallContext context)
        {
            var res = Data.Students.Where(s => s.UserName == request.UserName).FirstOrDefault();
            if (res == null)
            {
                return Task.FromResult(new StudentResponse { });
            }
            var studentRes = new StudentResponse
            {
                Student = res
            };
            return Task.FromResult(studentRes);
        }
        public override async Task GetAllStudent(QueryAllStudentRequest request, 
            IServerStreamWriter<StudentResponse> responseStream,
            ServerCallContext context)
        {
            // 之前整体传过去，现在可以模拟一个一个的传
            foreach(var student in Data.Students)
            {
                await responseStream.WriteAsync(new StudentResponse { Student = student });
            }
        }

        public override async Task<CommonResponse> UploadImg(IAsyncStreamReader<UploadImgRequest> requestStream,
            ServerCallContext context)
        {
            try
            {
                var tempData = new List<byte>();
                while (await requestStream.MoveNext())
                {
                    tempData.AddRange(requestStream.Current.Data);
                }
                Console.WriteLine($"接收到文件大小:{tempData.Count}bytes");

                using FileStream fs = new FileStream("test.jpg", FileMode.Create);
                fs.Write(tempData.ToArray(), 0, tempData.ToArray().Length);

                return new CommonResponse { Code = 0, Msg = "接收成功" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Code = -1, Msg = "接收失败" };
            }
        }

        public override async Task AddManyStudents(IAsyncStreamReader<AddStudentRequest> requestStream, IServerStreamWriter<StudentResponse> responseStream, ServerCallContext context)
        {
            while(await requestStream.MoveNext())
            {
                var student = requestStream.Current.Student;
                Data.Students.Add(student);

                await responseStream.WriteAsync(new StudentResponse
                {
                    Student = student
                }) ;
            }

        }
        [AllowAnonymous]
        public override Task<TokenResponse> GetToken(TokenRequest request, ServerCallContext context)
        {
            // 模拟登陆验证
            if(request.UserName!="Code综艺圈"||request.UserPwd!="admin123")
            {
                return Task.FromResult(new TokenResponse { Token = string.Empty });
            }
            // 这里可以根据需要将其权限放在Redis中，每次登陆时都重新存，即登陆获取最新权限
            // 这里模拟通过userId从数据库中获取分配的接口权限信息，这里存在内存中
            _permissionRequirement.Permissions = new List<PermissionData> {
                new PermissionData{ UserId="UserId123456",Url="/user.studentservice/getallstudent" },
                
                new PermissionData{ UserId="UserId123456",Url="/user.studentservice/addmanystudents" }
            };

            // 当登陆验证通过之后才获取对应的token
            string token = GenerateToken("UserId123456", request.UserName);
            // 返回效应结果
            return Task.FromResult(new TokenResponse { Token=token});
        }

        private string GenerateToken(string userId, string userName)
        {
            // 秘钥，这是生成Token需要秘钥，和Startup中用到的一致
            string secret = "TestSecretTestSecretTestSecretTestSecret";
            // 签发者，是由谁颁发的，和Startup中用到的一致
            string issuer = "TestgRPCIssuer";
            // 接受者，是给谁用的，和Startup中用到的一致
            string audience = "TestgRPCAudience";
            // 指定秘钥
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
            // 签名凭据，指定对应的签名算法，结合理论那块看哦~~~
            var sigingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            // 自定义payload信息，每一个claim代表一个属性键值对，就类似身份证上的姓名，出生年月一样
            var claims = new Claim[] { new Claim("userId", userId),
                new Claim("userName", userName),
                // claims中添加角色属性，这里的键一定要用微软封装好的ClaimTypes.Role
                new Claim(ClaimTypes.Role,"Admin"),
                new Claim(ClaimTypes.Role,"Maintain")
            };

            // 组装生成Token的数据
            SecurityToken securityToken = new JwtSecurityToken(
                    issuer: issuer,// 颁发者
                    audience: audience,// 接受者
                    claims: claims,//自定义的payload信息
                    signingCredentials: sigingCredentials,// 凭据
                    expires: DateTime.Now.AddMinutes(30) // 设置超时时间，30分钟之后过期
                );
            // 生成Token
            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }
    }
}
