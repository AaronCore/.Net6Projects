using System;
using Volo.Abp.Domain.Entities;

namespace VueAdmin.Domain.System
{
    public class AccountEntity : Entity<Guid>
    {
        /// <summary>
        /// 登录账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 角色Id
        /// </summary>
        public int RoleId { set; get; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// 编辑时间
        /// </summary>
        public DateTime? EditTime { get; set; }
        /// <summary>
        /// 编辑人
        /// </summary>
        public string Editor { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creater { get; set; }
    }
}
