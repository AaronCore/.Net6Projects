using System;
using Volo.Abp.Domain.Repositories;

namespace VueAdmin.Domain.System.Repositories
{
    /// <summary>
    /// IRoleRepository
    /// </summary>
    public interface IRoleRepository : IRepository<RoleEntity, Guid>
    {
    }
}
