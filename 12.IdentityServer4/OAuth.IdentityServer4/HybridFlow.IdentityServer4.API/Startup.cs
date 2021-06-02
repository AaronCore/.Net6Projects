using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HybridFlow.IdentityServer4.API
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            #region 1.用ids4包

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
              .AddIdentityServerAuthentication(JwtBearerDefaults.AuthenticationScheme, options =>
               {
                   options.Authority = "http://localhost:5001";
                   options.RequireHttpsMetadata = false;
                   options.ApiName = "api1";
                   options.ApiSecret = "apipassword"; //对应ApiResources中的密钥
               });
            services.AddAuthorization();
            #endregion

            #region 2.用自带的JwtBearer程序

            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //   .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            //   {
            //       // IdentityServer 地址
            //       options.Authority = "http://localhost:5001";
            //       //不需要https
            //       options.RequireHttpsMetadata = false;
            //       //这里要和 IdentityServer 定义的 api1 保持一致
            //       options.Audience = "api1";
            //   });

            #endregion

            #region 3.用自带的JwtBearer程序，并添加授权策略作用域

            //services.AddAuthentication("Bearer")
            //   .AddJwtBearer("Bearer", options =>
            //   {
            //       options.Authority = "http://localhost:5001";
            //       options.RequireHttpsMetadata = false;
            //       options.TokenValidationParameters = new TokenValidationParameters
            //       {
            //           ValidateAudience = false
            //       };
            //   });

            //// adds an authorization policy to make sure the token is for scope 'api1'
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("ApiScope", policy =>
            //    {
            //        policy.RequireAuthenticatedUser();
            //        policy.RequireClaim("scope", "api1");
            //    });
            //});

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello HybridFlow.IdentityServer4.API!");
                });
            });
        }
    }
}
