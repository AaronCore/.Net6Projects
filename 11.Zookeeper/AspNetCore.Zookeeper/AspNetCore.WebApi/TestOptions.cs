using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.WebApi
{
    public class TestOptions
    {
        /// <summary>
        /// 字符数据
        /// </summary>
        public string Value1 { get; set; }
        /// <summary>
        /// 整形数据
        /// </summary>
        public int Value2 { get; set; }
        /// <summary>
        /// 浮点型数据
        /// </summary>
        public decimal Value3 { get; set; }
        /// <summary>
        /// 布尔型数据
        /// </summary>
        public bool Value4 { get; set; }
        /// <summary>
        /// 日期数据
        /// </summary>
        public DateTime Value5 { get; set; }
    }
}
