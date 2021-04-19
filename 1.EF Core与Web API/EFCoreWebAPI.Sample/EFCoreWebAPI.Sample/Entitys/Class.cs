using System;
using System.Collections.Generic;

namespace EFCoreWebAPI.Sample.Entitys
{
    /// <summary>
    /// 班级表
    /// </summary>
    public class Class
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 班级名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 班级学生
        /// </summary>
        public virtual ICollection<Students> Students { get; set; } = new List<Students>();
    }
}
