using Grpc.Server.Demo.Permission;
using Grpc.Server.Demo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Server.Demo
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            #region 集成JWT
            // 将公共的信息提取出来，这里可以放到配置文件中，统一读取;以下直接在程序中写死了
            // 秘钥，这是生成Token需要秘钥，就是理论提及到签名那块的秘钥
            string secret = "TestSecretTestSecretTestSecretTestSecret";
            // 签发者，是由谁颁发的
            string issuer = "TestgRPCIssuer";
            // 接受者，是给谁用的
            string audience = "TestgRPCAudience";
            // 注册服务，显示指定为Bearer
            services.AddAuthentication("Bearer")
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // 是否验证秘钥
                        ValidateIssuerSigningKey = true,
                        // 指定秘钥
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                        // 是否验证颁发者
                        ValidateIssuer = true,
                        // 指定颁发者
                        ValidIssuer = issuer,
                        // 是否验证接受者
                        ValidateAudience = true,
                        // 指定接受者
                        ValidAudience = audience,
                        // 设置必须要有超时时间
                        RequireExpirationTime = true,
                        // 设置必须验证超时
                        ValidateLifetime = true,
                        // 将其赋值为0时，即设置有效时间到期，就马上失效
                        ClockSkew = TimeSpan.Zero
                    };
                });
            #endregion
            var permissionRequirement = new PermissionRequirement
            {
                Permissions = new List<PermissionData>()
            };
            //针对授权定义策略
            services.AddAuthorization(option =>
            {
                // 同样是策略模式，只是基于Requirement进行权限验证，验证逻辑自己写
                option.AddPolicy("Permission", p => p.Requirements.Add(permissionRequirement));
            });
            // 将权限验证的关键类注册，Jwt的策略模式指定为Requirements时就会自动执行该类方法
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            // 将permissionRequirement实例注册为单例模式，保证系统中就一个实例，方便权限数据共享
            services.AddSingleton(permissionRequirement);
            // 注册IHttpContextAccessor，后续可以通过它可以获取HttpContext，操作方便
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<StudentDemoService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
