using System;
using System.ComponentModel.DataAnnotations;

namespace EFCoreWebAPI.Sample.DtoModels
{
    /// <summary>
    /// 班级Dto
    /// </summary>
    public class ClassDto
    {
        /// <summary>
        /// 班级Id
        /// </summary>
        public Guid ClassId { get; set; }
        /// <summary>
        /// 班级名称
        /// </summary>
        public string ClassName { get; set; }
    }

    /// <summary>
    /// 班级添加、修改Dto
    /// </summary>
    public class AddOrUpdateClassDto
    {
        [Display(Name = "班级名称")]
        [Required(ErrorMessage = "{0}这个字段是必填的")]
        [MaxLength(10, ErrorMessage = "{0}的最大长度不可以超过{1}")]
        public string Name { get; set; }
    }
}
