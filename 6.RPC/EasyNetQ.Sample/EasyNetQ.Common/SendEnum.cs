using System;
using System.Collections.Generic;
using System.Text;

namespace EasyNetQ.Common
{
    public enum SendEnum
    {
        订阅模式 = 1,
        推送模式 = 2,
        主题路由模式 = 3
    }
}
