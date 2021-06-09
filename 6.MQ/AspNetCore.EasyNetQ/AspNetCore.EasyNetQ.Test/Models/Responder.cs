using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.EasyNetQ.Test
{
    public class Responder
    {
        public string Result { get; set; }
    }
    public class Requester
    {
        public string Data { get; set; }
    }
}
