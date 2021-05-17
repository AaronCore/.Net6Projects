using System;
using System.ComponentModel.DataAnnotations;

namespace SysEntity
{
    public class PersonEntity
    {
        /// <summary>
        /// 标识
        /// </summary>
        public Guid Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(50)]
        public string Phone { get; set; }
        public DateTime CreatDateTime { get; set; }
    }
}
