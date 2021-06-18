namespace VueAdmin.Application.Contracts.System.Menu
{
    public class MenuInput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { set; get; }
        /// <summary>
        /// 父级Id
        /// </summary>
        public string ParentId { set; get; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 菜单地址
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 菜单编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }
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
