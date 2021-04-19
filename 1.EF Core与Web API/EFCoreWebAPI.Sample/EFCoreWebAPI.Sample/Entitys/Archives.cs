using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFCoreWebAPI.Sample.Entitys
{
    /// <summary>
    /// 档案表
    /// </summary>
    public class Archives
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 档案编号
        /// </summary>
        public string No { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Comment("创建时间"), Column(TypeName = "datetime")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 学习信息
        /// </summary>
        public Guid StudentId { get; set; }
        public virtual Students Student { get; set; }
    }
}
