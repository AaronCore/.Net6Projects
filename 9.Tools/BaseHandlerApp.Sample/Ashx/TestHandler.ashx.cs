using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BaseHandlerApp.Sample.Ashx
{
    /// <summary>
    /// TestHandler 的摘要说明
    /// </summary>
    public class TestHandler : BaseHandler
    {
        //调用 /TestHandler.ashx?action=Show
        public TestHandler()
        {
            //修改请求为get 方式
            base._httpReuqest = base.Context.Request.Form;
        }

        public void Show()
        {
            PrintSuccessJson("调用Show方法成功！");
        }
    }
}