using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VueAdmin.Application.Contracts.System.Account;
using VueAdmin.Application.Contracts.System.Role;
using VueAdmin.Common.Base;
using VueAdmin.Common.Extensions;
using VueAdmin.Domain.System;
using VueAdmin.Domain.System.Repositories;

namespace VueAdmin.Application.System.Account
{
    public class AccountService : VueAdminApplicationService, IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="pageIndex">分页下标</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="query">查询值</param>
        /// <returns></returns>
        public async Task<ServiceResult<PagedList<AccountOut>>> QueryList(int pageIndex, int pageSize, string query)
        {
            var result = new ServiceResult<PagedList<AccountOut>>();

            var total = await _accountRepository.GetCountAsync();

            Expression<Func<AccountEntity, bool>> where = e => true;
            if (!string.IsNullOrWhiteSpace(query))
            {
                where = where.And(p => p.Account.Contains(query));
            }
            var roles = _accountRepository.Where(where).OrderByDescending(p => p.CreateTime).PageByIndex(pageIndex, pageSize);
            var list = ObjectMapper.Map<IEnumerable<AccountEntity>, IEnumerable<AccountOut>>(roles);

            result.IsSuccess(new PagedList<AccountOut>(total.TryToInt(), list.ToList()));
            return result;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ServiceResult> AddAsync(AccountInput input)
        {
            var result = new ServiceResult();

            var entity = ObjectMapper.Map<AccountInput, AccountEntity>(input);
            entity.CreateTime = DateTime.Now;
            entity.Creater = "admin";

            await _accountRepository.InsertAsync(entity, true);

            result.IsSuccess();
            return result;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ServiceResult> UpdateAsync(string id, AccountInput input)
        {
            var result = new ServiceResult();

            var entity = await _accountRepository.FindAsync(p => p.Id.ToString() == id);
            entity.Account = input.Account;
            entity.Sort = input.Sort;
            entity.Enabled = input.Enabled;
            entity.EditTime = DateTime.Now;
            entity.Editor = "admin";

            await _accountRepository.UpdateAsync(entity, true);

            result.IsSuccess();
            return result;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ServiceResult<AccountOut>> GetRoleAsync(string id)
        {
            var result = new ServiceResult<AccountOut>();

            var entity = await _accountRepository.FindAsync(p => p.Id.ToString() == id);
            var model = ObjectMapper.Map<AccountEntity, AccountOut>(entity);

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

            await _accountRepository.DeleteAsync(p => ids.Contains(p.Id.ToString()), true);

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

            var roles = _accountRepository.Where(p => ids.Contains(p.Id.ToString())).ToList();
            foreach (var item in roles)
            {
                item.Enabled = !item.Enabled;
                await _accountRepository.UpdateAsync(item);
            }

            return result;
        }
    }
}
