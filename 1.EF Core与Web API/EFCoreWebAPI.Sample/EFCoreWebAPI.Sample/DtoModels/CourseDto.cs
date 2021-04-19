using System;

namespace EFCoreWebAPI.Sample.DtoModels
{
    /// <summary>
    /// 课程Dto
    /// </summary>
    public class CourseDto
    {
        /// <summary>
        /// 课程Id
        /// </summary>
        public Guid CourseId { get; set; }
        /// <summary>
        /// 课程名称
        /// </summary>
        public string CourseName { get; set; }
    }
}
