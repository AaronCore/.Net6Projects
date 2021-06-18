using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using VueAdmin.Application.Contracts.System.Role;
using VueAdmin.Application.System.Role;
using VueAdmin.Common.Base;
using VueAdmin.Domain.Shared;

namespace VueAdmin.HttpApi.Controllers
{
    /// <summary>
    /// Role接口
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(GroupName = VueAdminConsts.Grouping.GroupName_v1)]
    public class RoleController : AbpController
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
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
            var result = await _roleService.QueryList(pageIndex, pageSize, query);
            return result;
        }

        /// <summary>
        /// 提交Role
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("submitRole")]
        public async Task<ServiceResult> SubmitAsync([FromBody] RoleInput input)
        {
            if (!string.IsNullOrWhiteSpace(input.Id))
            {
                return await _roleService.UpdateAsync(input.Id, input);
            }
            else
            {
                return await _roleService.AddAsync(input);
            }
        }

        /// <summary>
        /// 获取Role
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getRole")]
        public async Task<ServiceResult> GetRoleAsync(string id)
        {
            var result = await _roleService.GetRoleAsync(id);
            return result;
        }

        /// <summary>
        /// 批量删除Role
        /// </summary>
        /// <param name="ids">主键</param>
        /// <returns></returns>
        [HttpGet]
        [Route("deleteRole")]
        public async Task<ServiceResult> DeleteAsync(string[] ids)
        {
            var result = await _roleService.DeleteAsync(ids);
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
            var result = await _roleService.EditRangeEnabledAsync(ids);
            return result;
        }
    }
}