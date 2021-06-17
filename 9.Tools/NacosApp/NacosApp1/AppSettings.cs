using System.Collections.Generic;

namespace NacosApp1
{
    public class AppSettings
    {
        public string Str { get; set; }
        public int Num { get; set; }
        public List<int> Arr { get; set; }
        public SubObj SubObj { get; set; }
    }
    public class SubObj
    {
        public string A { get; set; }
    }
}
