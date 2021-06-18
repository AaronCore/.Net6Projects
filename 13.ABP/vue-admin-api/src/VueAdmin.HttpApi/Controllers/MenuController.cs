using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using VueAdmin.Application.Contracts.System.Menu;
using VueAdmin.Application.System.Menu;
using VueAdmin.Common.Base;
using VueAdmin.Domain.Shared;

namespace VueAdmin.HttpApi.Controllers
{
    /// <summary>
    /// Menu接口
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(GroupName = VueAdminConsts.Grouping.GroupName_v1)]
    public class MenuController : AbpController
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
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
            var result = await _menuService.QueryList(pageIndex, pageSize, query);
            return result;
        }

        /// <summary>
        /// 提交Menu
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("submitMenu")]
        public async Task<ServiceResult> SubmitAsync([FromBody] MenuInput input)
        {
            if (!string.IsNullOrWhiteSpace(input.Id))
            {
                // 修改
                return await _menuService.UpdateAsync(input.Id, input);
            }
            // 添加
            return await _menuService.AddAsync(input);
        }

        /// <summary>
        /// 获取Menu
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getMenu")]
        public async Task<ServiceResult> GetMenuAsync(string id)
        {
            var result = await _menuService.GetMenuAsync(id);
            return result;
        }

        /// <summary>
        /// 批量删除Menu
        /// </summary>
        /// <param name="ids">主键</param>
        /// <returns></returns>
        [HttpGet]
        [Route("deleteMenu")]
        public async Task<ServiceResult> DeleteAsync(string[] ids)
        {
            var result = await _menuService.DeleteAsync(ids);
            return result;
        }

        /// <summary>
        /// 修改Menu状态
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="enabled">状态</param>
        /// <returns></returns>
        [HttpGet]
        [Route("editEnabled")]
        public async Task<ServiceResult> EditEnabledAsync(string id, bool enabled)
        {
            var result = await _menuService.EditEnabledAsync(id, enabled);
            return result;
        }
    }
}