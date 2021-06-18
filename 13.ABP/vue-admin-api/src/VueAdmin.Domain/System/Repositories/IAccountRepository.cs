using System;
using Volo.Abp.Domain.Repositories;

namespace VueAdmin.Domain.System.Repositories
{
    /// <summary>
    /// IAccountRepository
    /// </summary>
    public interface IAccountRepository : IRepository<AccountEntity, Guid>
    {

    }
}