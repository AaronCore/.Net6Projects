using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using VueAdmin.Application.Contracts.System.Account;
using VueAdmin.Application.System.Account;
using VueAdmin.Common.Base;
using VueAdmin.Domain.Shared;

namespace VueAdmin.HttpApi.Controllers
{
    /// <summary>
    /// Account接口
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(GroupName = VueAdminConsts.Grouping.GroupName_v1)]
    public class AccountController : AbpController
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="pageIndex">分页下标</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="query">查询条件</param>
        /// <returns></returns>
        [HttpGet]
        [Route("query")]
        public async Task<ServiceResult> QueryListAsync(int pageIndex, int pageSize, string query)
        {
            var result = await _accountService.QueryList(pageIndex, pageSize, query);
            return result;
        }

        /// <summary>
        /// 提交Account
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("submitAccount")]
        public async Task<ServiceResult> SubmitAsync([FromBody] AccountInput input)
        {
            if (!string.IsNullOrWhiteSpace(input.Id))
            {
                return await _accountService.UpdateAsync(input.Id, input);
            }
            else
            {
                return await _accountService.AddAsync(input);
            }
        }

        /// <summary>
        /// 获取Account
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getAccount")]
        public async Task<ServiceResult> GetRoleAsync(string id)
        {
            var result = await _accountService.GetRoleAsync(id);
            return result;
        }

        /// <summary>
        /// 批量删除Account
        /// </summary>
        /// <param name="ids">主键</param>
        /// <returns></returns>
        [HttpGet]
        [Route("deleteAccount")]
        public async Task<ServiceResult> DeleteAsync(string[] ids)
        {
            var result = await _accountService.DeleteAsync(ids);
            return result;
        }

        /// <summary>
        /// 批量修改Role状态
        /// </summary>
        /// <param name="ids">主键</param>
        /// <returns></returns>
        [HttpGet]
        [Route("editEnabled")]
        public async Task<ServiceResult> EditRangeEnabledAsync(string[] ids)
        {
            var result = await _accountService.EditRangeEnabledAsync(ids);
            return result;
        }
    }
}