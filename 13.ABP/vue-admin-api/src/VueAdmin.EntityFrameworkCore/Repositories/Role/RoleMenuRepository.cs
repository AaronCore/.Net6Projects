using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VueAdmin.Domain.System;
using VueAdmin.Domain.System.Repositories;
using VueAdmin.EntityFrameworkCore.EntityFrameworkCore;

namespace VueAdmin.EntityFrameworkCore.Repositories.Role
{
    public class RoleMenuRepository : EfCoreRepository<VueAdminDbContext, RoleMenuEntity, Guid>, IRoleMenuRepository
    {
        public RoleMenuRepository(IDbContextProvider<VueAdminDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
    }
}
