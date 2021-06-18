using System;
using Volo.Abp.Domain.Repositories;

namespace VueAdmin.Domain.System.Repositories
{
    /// <summary>
    /// IMenuRepository
    /// </summary>
    public interface IMenuRepository : IRepository<MenuEntity, Guid>
    {

    }
}
