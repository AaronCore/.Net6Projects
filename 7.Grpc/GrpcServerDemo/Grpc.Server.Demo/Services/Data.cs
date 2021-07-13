using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grpc.Server.Demo.Services
{
    public static class Data
    {
        public static List<Student> Students = new List<Student>
        {
            new Student{ UserName="Zoe", Age=18 , Addr="远方" },
            new Student{ UserName="Code综艺圈", Age=18 , Addr="微信公众号了解更多" },
            new Student{ UserName="Test", Age=18 , Addr="Test" }
        };
    }
}
