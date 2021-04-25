using System;

namespace NpoiMapper.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                PersonTest.Test2();
                Console.WriteLine("执行完成");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.Read();
        }
    }
}
