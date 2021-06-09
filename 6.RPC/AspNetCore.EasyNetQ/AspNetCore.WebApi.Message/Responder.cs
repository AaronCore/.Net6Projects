using AspNetCore.EasyNetQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.WebApi.Message
{
    public class Responder
    {
        public string Result { get; set; }
    }
    public class Requester
    {
        public string Data { get; set; }
    }
    public class EasyNetQResponder : IEasyNetQResponder<Requester, Responder>
    {
        public Task<Responder> RespondAsync(Requester request)
        {
            Console.WriteLine("EasyNetQResponder:" + request.Data);
            return Task.FromResult(new Responder() { Result = "EasyNetQResponder:" + request.Data });
        }
    }
}
