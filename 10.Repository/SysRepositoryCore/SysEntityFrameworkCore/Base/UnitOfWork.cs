using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SysEntityFrameworkCore
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SysDbContext _dbContext;
        public UnitOfWork(SysDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        /// <summary>
        /// 提交
        /// </summary>
        public void Commit()
        {
            _dbContext.SaveChanges();
        }
        /// <summary>
        /// 异步提交
        /// </summary>
        /// <returns></returns>
        public async Task CommitAsync()
        {
            await _dbContext.SaveChangesAsync(CancellationToken.None);
        }
        /// <summary>
        /// 内存回收
        /// </summary>
        public void Dispose()
        {
            _dbContext.Dispose();
            GC.Collect();
        }
    }
}
