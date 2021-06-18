namespace VueAdmin.Application.Contracts.System.Role
{
    public class RoleInput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { set; get; }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }
    }
}
