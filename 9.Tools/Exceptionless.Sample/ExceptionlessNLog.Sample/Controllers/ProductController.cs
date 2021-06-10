using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exceptionless.Sample.Controllers
{
    [Route("productapi/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger _logger;
        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }

        [HttpGet("exceptiontest")]
        public string ExceptionTest()
        {
            try
            {
                var sss = Convert.ToInt32("AAA");
                //throw new Exception("发生了未知的异常");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{HttpContext.Connection.RemoteIpAddress}调用了 productapi/product/exceptiontest 接口返回了失败");
            }
            return "调用失败";
        }
    }
}
