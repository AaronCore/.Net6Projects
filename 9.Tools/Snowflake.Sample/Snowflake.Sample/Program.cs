using System;
using Snowflake.Core;

namespace Snowflake.Sample
{
    class Program
    {
        // 布式全局唯一ID算法
        static void Main(string[] args)
        {
            var worker = new IdWorker(1, 1);
            long id = worker.NextId();
            Console.WriteLine(id);
        }
    }
}
