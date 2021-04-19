using System;

namespace EFCoreWebAPI.Sample.DtoModels
{
    /// <summary>
    /// 学生Dto
    /// </summary>
    public class StudentDto : ClassDto
    {
        /// <summary>
        /// 学生Id
        /// </summary>
        public Guid StudentId { get; set; }
        /// <summary>
        /// 学生编号
        /// </summary>
        public string StudentNo { get; set; }
        /// <summary>
        /// 学生名称
        /// </summary>
        public string StudentName { get; set; }
        /// <summary>
        /// 学生性别
        /// </summary>
        public string Gender { get; set; }
        /// <summary>
        /// 学生生日
        /// </summary>
        public DateTime DateOfBirth { get; set; }
    }
    /// <summary>
    /// 学生添加、修改Dto
    /// </summary>
    public class AddOrUpdateStudentDto
    {
        /// <summary>
        /// 班级Id
        /// </summary>
        public Guid ClassId { get; set; }
        /// <summary>
        /// 学生编号
        /// </summary>
        public string StudentNo { get; set; }
        /// <summary>
        /// 学生名称
        /// </summary>
        public string StudentName { get; set; }
        /// <summary>
        /// 学生性别
        /// </summary>
        public string Gender { get; set; }
        /// <summary>
        /// 学生生日
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// 学生课程
        /// </summary>
        public Guid[] StudentCourseIds { get; set; }
    }
    /// <summary>
    /// 学生课程Dto
    /// </summary>
    public class StudentCourseDto
    {
        /// <summary>
        /// 学生Id
        /// </summary>
        public Guid StudentId { get; set; }
        /// <summary>
        /// 课程Id
        /// </summary>
        public Guid CourseId { get; set; }
    }
}
