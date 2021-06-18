using System.Threading.Tasks;
using VueAdmin.Application.Contracts.System.Account;
using VueAdmin.Common.Base;

namespace VueAdmin.Application.System.Account
{
    public interface IAccountService
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="pageIndex">分页下标</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="query">查询值</param>
        /// <returns></returns>
        Task<ServiceResult<PagedList<AccountOut>>> QueryList(int pageIndex, int pageSize, string query);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ServiceResult> AddAsync(AccountInput input);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ServiceResult> UpdateAsync(string id, AccountInput input);

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ServiceResult<AccountOut>> GetRoleAsync(string id);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<ServiceResult> DeleteAsync(string[] ids);

        /// <summary>
        /// 批量修改Role状态
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<ServiceResult> EditRangeEnabledAsync(string[] ids);
    }
}