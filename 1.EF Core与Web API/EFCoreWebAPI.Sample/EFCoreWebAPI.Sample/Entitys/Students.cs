using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFCoreWebAPI.Sample.Entitys
{
    /// <summary>
    /// 学生表
    /// </summary>
    [Comment("学生表")]
    public class Students
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Comment("主键")]
        public Guid Id { get; set; }
        /// <summary>
        /// 学生编号
        /// </summary>
        [Required, MaxLength(20), Comment("学生编号")]
        public string No { get; set; }
        /// <summary>
        /// 学生姓名
        /// </summary>
        [Comment("学生姓名")]
        public string Name { get; set; }
        /// <summary>
        /// 学生性别
        /// </summary>
        [Comment("学生性别")]
        public Gender Gender { get; set; }
        /// <summary>
        /// 学生生日
        /// </summary>
        [Comment("学生生日"), Column(TypeName = "datetime")]
        public DateTime DateOfBirth { get; set; }
        /// <summary>
        /// 班级
        /// </summary>
        public Guid ClassId { get; set; }
        public virtual Class Class { get; set; }
        /// <summary>
        /// 学生档案
        /// </summary>
        public Guid ArchiveId { get; set; }
        public virtual Archives Archive { get; set; }
        /// <summary>
        /// 学生课程
        /// </summary>
        public virtual List<StudentCourses> StudentCourses { get; set; } = new List<StudentCourses>();
    }
}
