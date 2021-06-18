using log4net;
using Microsoft.AspNetCore.Mvc.Filters;

namespace VueAdmin.HttpApi.Host.Filters
{
    public class VueAdminExceptionFilter : IExceptionFilter
    {
        private readonly ILog _log;

        public VueAdminExceptionFilter()
        {
            _log = LogManager.GetLogger(typeof(VueAdminExceptionFilter));
        }

        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public void OnException(ExceptionContext context)
        {
            // 错误日志记录
            _log.Error($"{context.HttpContext.Request.Path}|{context.Exception.Message}", context.Exception);
        }
    }
}
