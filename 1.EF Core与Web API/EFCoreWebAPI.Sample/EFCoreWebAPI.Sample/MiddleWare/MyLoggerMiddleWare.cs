using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace EFCoreWebAPI.Sample.MiddleWare
{
    public static class MyLoggerMiddlerWareExtension
    {
        public static IApplicationBuilder MyUseLogger(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MyLoggerMiddleWare>();
        }
    }
    public class MyLoggerMiddleWare
    {
        private RequestDelegate _next;

        public MyLoggerMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.ContentType = "text/plain;charset=utf-8;";

            await context.Response.WriteAsync("this is logger before;\n");

            // Call the next delegate/middleware in the pipeline
            await this._next(context);

            await context.Response.WriteAsync("\nthis is logger after;");

        }
    }
}
