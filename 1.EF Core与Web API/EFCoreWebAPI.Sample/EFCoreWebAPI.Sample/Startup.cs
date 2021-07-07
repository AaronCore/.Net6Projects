using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json.Serialization;
using EFCoreWebAPI.Sample.DataContext;
using EFCoreWebAPI.Sample.Extensions;
using Microsoft.Extensions.DependencyInjection;
using EFCoreWebAPI.Sample.MiddleWare;

namespace EFCoreWebAPI.Sample
{
    public class Startup
    {
        public static readonly ILoggerFactory EfLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "EFCoreWebAPI.Sample API文档",
                        Description = "EFCoreWebAPI.Sample API帮助文档",
                        TermsOfService = new Uri("https://cn.bing.com"),
                        Contact = new OpenApiContact()
                        {
                            Name = "Sample",
                            Email = "Sample@163.com",
                            Url = new Uri("https://cn.bing.com")
                        }
                    }
                );
                // 为 Swagger JSON and UI设置xml文档注释路径
                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
                var xmlPath = Path.Combine(basePath, "EFCoreWebAPI.Sample.xml");
                c.IncludeXmlComments(xmlPath);
            });

            services.AddControllers(setup =>
            {
                setup.ReturnHttpNotAcceptable = true;
            }).AddNewtonsoftJson(setup =>
            {
                setup.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            })
            //.AddXmlDataContractSerializerFormatters()
            .ConfigureApiBehaviorOptions(setup =>
            {
                setup.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Type = "https://cn.bing.com",
                        Title = "有错误！！！",
                        Status = StatusCodes.Status422UnprocessableEntity,
                        Detail = "请看详细信息",
                        Instance = context.HttpContext.Request.Path
                    };

                    problemDetails.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);

                    return new UnprocessableEntityObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json" }
                    };
                };
            });

            // 数据库链接
            var connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<DataDbContext>(options => options
                .UseLazyLoadingProxies()
                .UseLoggerFactory(EfLoggerFactory)
                .EnableDetailedErrors()
                .UseSqlServer(connection)
            );

            // AutoMapper映射
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            //services.AddHttpContextAccessorExtension();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sample v1"));
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("Unexpected Error!");
                    });
                });
            }

            //app.UseStaticHttpContext();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            //app.MyUseLogger();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }


    }
}
