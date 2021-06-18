using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using VueAdmin.Application.Contracts.System.Role;
using VueAdmin.Common.Base;
using VueAdmin.Common.Extensions;
using VueAdmin.Domain.System;
using VueAdmin.Domain.System.Repositories;

namespace VueAdmin.Application.System.Role
{
    public class RoleService : VueAdminApplicationService, IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="pageIndex">分页下标</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="query">查询值</param>
        /// <returns></returns>
        public async Task<ServiceResult<PagedList<RoleOut>>> QueryList(int pageIndex, int pageSize, string query)
        {
            var result = new ServiceResult<PagedList<RoleOut>>();

            var total = await _roleRepository.GetCountAsync();

            Expression<Func<RoleEntity, bool>> where = e => true;
            if (!string.IsNullOrWhiteSpace(query))
            {
                where = where.And(p => p.Name.Contains(query));
            }
            var roles = _roleRepository.Where(where).OrderByDescending(p => p.CreateTime).PageByIndex(pageIndex, pageSize);
            var list = ObjectMapper.Map<IEnumerable<RoleEntity>, IEnumerable<RoleOut>>(roles);

            result.IsSuccess(new PagedList<RoleOut>(total.TryToInt(), list.ToList()));
            return result;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ServiceResult> AddAsync(RoleInput input)
        {
            var result = new ServiceResult();

            var entity = ObjectMapper.Map<RoleInput, RoleEntity>(input);
            entity.CreateTime = DateTime.Now;
            entity.Creater = "admin";

            await _roleRepository.InsertAsync(entity, true);

            result.IsSuccess();
            return result;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ServiceResult> UpdateAsync(string id, RoleInput input)
        {
            var result = new ServiceResult();

            var entity = await _roleRepository.FindAsync(p => p.Id.ToString() == id);
            entity.Name = input.Name;
            entity.Sort = input.Sort;
            entity.Enabled = input.Enabled;
            entity.EditTime = DateTime.Now;
            entity.Editor = "admin";

            await _roleRepository.UpdateAsync(entity, true);

            result.IsSuccess();
            return result;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ServiceResult<RoleOut>> GetRoleAsync(string id)
        {
            var result = new ServiceResult<RoleOut>();

            var entity = await _roleRepository.FindAsync(p => p.Id.ToString() == id);
            var model = ObjectMapper.Map<RoleEntity, RoleOut>(entity);

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
            var result = new ServiceResult<RoleOut>();

            await _roleRepository.DeleteAsync(p => ids.Contains(p.Id.ToString()), true);

            result.IsSuccess();
            return result;
        }

        /// <summary>
        /// 批量修改Role状态
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<ServiceResult> EditRangeEnabledAsync(string[] ids)
        {
            var result = new ServiceResult<RoleOut>();

            var roles = _roleRepository.Where(p => ids.Contains(p.Id.ToString())).ToList();
            foreach (var item in roles)
            {
                item.Enabled = !item.Enabled;
                await _roleRepository.UpdateAsync(item);
            }

            return result;
        }
    }
}
