using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysEntity
{
    public class DefaultEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid Id { set; get; }

        public int? Sort { set; get; }
        public bool Enabled { set; get; }
        public DateTime? CreateTime { set; get; }
        public string CreateOperator { set; get; }
        public DateTime? ModifyTime { set; get; }
        public string ModifyOperator { set; get; }
    }
}
