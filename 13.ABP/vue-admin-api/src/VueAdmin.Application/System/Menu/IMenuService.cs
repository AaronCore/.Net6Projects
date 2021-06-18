using System.Threading.Tasks;
using VueAdmin.Application.Contracts.System.Menu;
using VueAdmin.Common.Base;

namespace VueAdmin.Application.System.Menu
{
    public interface IMenuService
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="pageIndex">分页下标</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="query">查询值</param>
        /// <returns></returns>
        Task<ServiceResult<PagedList<MenuOut>>> QueryList(int pageIndex, int pageSize, string query);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ServiceResult> AddAsync(MenuInput input);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ServiceResult> UpdateAsync(string id, MenuInput input);

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ServiceResult<MenuOut>> GetMenuAsync(string id);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<ServiceResult> DeleteAsync(string[] ids);

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        Task<ServiceResult> EditEnabledAsync(string id, bool enabled);
    }
}
