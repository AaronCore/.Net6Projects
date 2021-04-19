using System;

namespace EFCoreWebAPI.Sample.Entitys
{
    /// <summary>
    /// 学生课程表
    /// </summary>
    public class StudentCourses
    {
        /// <summary>
        /// 学生信息
        /// </summary>
        public Guid StudentId { get; set; }
        public virtual Students Student { get; set; }
        /// <summary>
        /// 课程信息
        /// </summary>
        public Guid CourseId { get; set; }
        public virtual Courses Course { get; set; }
    }
}
