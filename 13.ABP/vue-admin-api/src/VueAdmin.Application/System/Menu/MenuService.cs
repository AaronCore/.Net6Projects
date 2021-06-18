using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VueAdmin.Application.Contracts.System.Menu;
using VueAdmin.Common.Base;
using VueAdmin.Common.Extensions;
using VueAdmin.Domain.System;
using VueAdmin.Domain.System.Repositories;

namespace VueAdmin.Application.System.Menu
{
    public class MenuService : VueAdminApplicationService, IMenuService
    {
        private readonly IMenuRepository _menuRepository;

        public MenuService(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="pageIndex">分页下标</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="query">查询值</param>
        /// <returns></returns>
        public async Task<ServiceResult<PagedList<MenuOut>>> QueryList(int pageIndex, int pageSize, string query)
        {
            var result = new ServiceResult<PagedList<MenuOut>>();

            var total = await _menuRepository.GetCountAsync();

            Expression<Func<MenuEntity, bool>> where = e => true;
            if (!string.IsNullOrWhiteSpace(query))
            {
                where = where.And(p => p.Name.Contains(query));
            }
            var menus = _menuRepository.Where(where).OrderByDescending(p => p.CreateTime).PageByIndex(pageIndex, pageSize);
            var list = ObjectMapper.Map<IEnumerable<MenuEntity>, IEnumerable<MenuOut>>(menus);

            result.IsSuccess(new PagedList<MenuOut>(total.TryToInt(), list.ToList()));
            return result;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ServiceResult> AddAsync(MenuInput input)
        {
            var result = new ServiceResult();

            var entity = ObjectMapper.Map<MenuInput, MenuEntity>(input);
            entity.CreateTime = DateTime.Now;
            entity.Creater = "admin";

            await _menuRepository.InsertAsync(entity, true);

            result.IsSuccess();
            return result;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ServiceResult> UpdateAsync(string id, MenuInput input)
        {
            var result = new ServiceResult();

            var entity = await _menuRepository.FindAsync(p => p.Id.ToString() == id);
            entity.ParentId = input.ParentId;
            entity.Name = input.Name;
            entity.Path = input.Path;
            entity.Code = input.Code;
            entity.Icon = input.Icon;
            entity.Sort = input.Sort;
            entity.Enabled = input.Enabled;
            entity.EditTime = DateTime.Now;
            entity.Editor = "admin";

            await _menuRepository.UpdateAsync(entity, true);

            result.IsSuccess();
            return result;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ServiceResult<MenuOut>> GetMenuAsync(string id)
        {
            var result = new ServiceResult<MenuOut>();

            var entity = await _menuRepository.FindAsync(p => p.Id.ToString() == id);
            var model = ObjectMapper.Map<MenuEntity, MenuOut>(entity);

            result.IsSuccess(model);
            return result;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<ServiceResult> DeleteAsync(string[] ids)
        {
            var result = new ServiceResult<MenuOut>();

            await _menuRepository.DeleteAsync(p => ids.Contains(p.Id.ToString()), true);

            result.IsSuccess();
            return result;
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public async Task<ServiceResult> EditEnabledAsync(string id, bool enabled)
        {
            var result = new ServiceResult<MenuOut>();

            var entity = await _menuRepository.FindAsync(p => p.Id.ToString() == id);
            entity.Enabled = enabled;
            await _menuRepository.UpdateAsync(entity, true);

            result.IsSuccess();
            return result;
        }
    }
}
