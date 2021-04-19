using System;
using System.Collections.Generic;

namespace EFCoreWebAPI.Sample.Entitys
{
    /// <summary>
    /// 课程表
    /// </summary>
    public class Courses
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 课程名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 课程学生
        /// </summary>
        public virtual ICollection<StudentCourses> StudentCourses { get; set; } = new List<StudentCourses>();
    }
}
