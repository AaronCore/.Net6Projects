using System;
using Volo.Abp.Domain.Repositories;

namespace VueAdmin.Domain.System.Repositories
{
    /// <summary>
    /// IRoleMenuRepository
    /// </summary>
    public interface IRoleMenuRepository : IRepository<RoleMenuEntity, Guid>
    {
    }
}
