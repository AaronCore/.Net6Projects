using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreWebAPI.Sample.MiddleWare
{
    public interface IMyPrint
    {
        void WriteMessage(string message);
    }
    public class MyPrint : IMyPrint
    {
        public void WriteMessage(string message)
        {
            Console.WriteLine($"MyPrint.WriteMessage Message: {message}");
        }
    }
}
