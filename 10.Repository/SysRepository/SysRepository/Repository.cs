using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SysEntityFrameworkCore;

namespace SysRepository
{
    public class Repository<T> : IRepository<T> where T : class, new()
    {
        private readonly SysDbContext _dbContext;
        public Repository(SysDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region 固定公用帮助，含事务

        /// <summary>
        /// 数据上下文
        /// </summary>
        public DbContext Context
        {
            get
            {
                _dbContext.Configuration.ValidateOnSaveEnabled = false;
                return _dbContext;
            }
        }
        /// <summary>
        /// 公用泛型处理属性
        /// 注:所有泛型操作的基础
        /// </summary>
        public DbSet<T> dbSet
        {
            get { return Context.Set<T>(); }
        }
        /// <summary>
        /// 事务
        /// </summary>
        private DbContextTransaction _transaction = null;
        /// <summary>
        /// 开始事务
        /// </summary>
        public DbContextTransaction Transaction
        {
            get
            {
                if (_transaction == null)
                {
                    _transaction = this.Context.Database.BeginTransaction();
                }
                return _transaction;
            }
            set { _transaction = value; }
        }
        /// <summary>
        /// 事务状态
        /// </summary>
        public bool Committed { get; set; }
        /// <summary>
        /// 异步锁定
        /// </summary>
        private readonly object sync = new object();
        /// <summary>
        /// 提交事务
        /// </summary>
        public void Commit()
        {
            if (!Committed)
            {
                lock (sync)
                {
                    if (_transaction != null)
                    {
                        _transaction.Commit();
                    }
                }
                Committed = true;
            }
        }
        /// <summary>
        /// 回滚事务
        /// </summary>
        public void Rollback()
        {
            Committed = false;
            if (_transaction != null)
            {
                _transaction.Rollback();
            }
        }

        #endregion

        #region 添加

        public virtual bool AddEntity(T entity, bool isCommit = true)
        {
            dbSet.Add(entity);
            return !isCommit || Context.SaveChanges() > 0;
        }
        public virtual bool AddEntityRange(IEnumerable<T> entitys, bool isCommit = true)
        {
            dbSet.AddRange(entitys);
            return !isCommit || Context.SaveChanges() > 0;
        }

        public virtual async Task<bool> AddEntityAsync(T entity, bool isCommit = true)
        {
            dbSet.Add(entity);
            if (!isCommit)
            {
                return false;
            }
            return await Context.SaveChangesAsync() > 0;
        }
        public virtual async Task<bool> AddEntityRangeAsync(IEnumerable<T> entitys, bool isCommit = true)
        {
            dbSet.AddRange(entitys);
            if (!isCommit)
            {
                return false;
            }
            return await Context.SaveChangesAsync() > 0;
        }

        #endregion

        #region 修改

        public virtual bool UpdateEntity(T entity, bool isCommit = true)
        {
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            Context.Entry(entity).State = EntityState.Modified;
            return !isCommit || Context.SaveChanges() > 0;
        }
        public virtual bool UpdateEntityRange(IEnumerable<T> entitys, bool isCommit = true)
        {
            foreach (var entity in entitys)
            {
                if (Context.Entry(entity).State == EntityState.Detached)
                {
                    dbSet.Attach(entity);
                }
                Context.Entry(entity).State = EntityState.Modified;
            }
            return !isCommit || Context.SaveChanges() > 0;
        }

        public virtual async Task<bool> UpdateAsync(T entity, bool isCommit = true)
        {
            if (!isCommit)
            {
                return false;
            }
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            Context.Entry(entity).State = EntityState.Modified;
            return await Context.SaveChangesAsync() > 0;
        }
        public virtual async Task<bool> UpdateEntityRangeAsync(IEnumerable<T> entities, bool isCommit = true)
        {
            if (!isCommit)
            {
                return false;
            }
            foreach (var entity in entities)
            {
                if (Context.Entry(entity).State == EntityState.Detached)
                {
                    dbSet.Attach(entity);
                }
                Context.Entry(entity).State = EntityState.Modified;
            }
            return await Context.SaveChangesAsync() > 0;
        }

        #endregion

        #region 删除

        public virtual bool DelEntity(T entity, bool isCommit = true)
        {
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);
            return !isCommit || Context.SaveChanges() > 0;
        }
        public virtual bool DelEntityRange(Expression<Func<T, bool>> filter, bool isCommit = true)
        {
            var entitys = dbSet.Where(filter);
            dbSet.RemoveRange(entitys);
            return !isCommit || Context.SaveChanges() > 0;
        }

        public virtual async Task<bool> DelEntityAsync(T entity, bool isCommit = true)
        {
            if (!isCommit)
            {
                return false;
            }
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);
            return await Context.SaveChangesAsync() > 0;
        }
        public virtual async Task<bool> DelEntityRangeAsync(Expression<Func<T, bool>> filter, bool isCommit = true)
        {
            if (!isCommit)
            {
                return false;
            }
            var entitys = dbSet.Where(filter);
            dbSet.RemoveRange(entitys);
            return await Context.SaveChangesAsync() > 0;
        }

        #endregion

        #region 查询

        public virtual T Find(object id)
        {
            return dbSet.Find(id);
        }
        public virtual IEnumerable<T> GetEntityAll()
        {
            return dbSet.ToList();
        }
        public virtual IEnumerable<T> GetEntityAll(Expression<Func<T, bool>> where)
        {
            return dbSet.Where(where).ToList();
        }
        public virtual T GetEntity(Expression<Func<T, bool>> where)
        {
            return dbSet.Where(where).FirstOrDefault();
        }
        public virtual bool AnyEntity(Expression<Func<T, bool>> where)
        {
            return dbSet.Any(where);
        }

        public virtual async Task<T> FindAsync(object id)
        {
            return await dbSet.FindAsync(id);
        }
        public virtual async Task<IEnumerable<T>> GetEntityAllAsync()
        {
            return await dbSet.ToListAsync();
        }
        public virtual async Task<IEnumerable<T>> GetEntityAllAsync(Expression<Func<T, bool>> where)
        {
            return await dbSet.Where(where).ToListAsync();
        }
        public virtual async Task<T> GetEntityAsync(Expression<Func<T, bool>> where)
        {
            return await dbSet.Where(where).FirstOrDefaultAsync();
        }
        public virtual async Task<bool> AnyEntityAsync(Expression<Func<T, bool>> where)
        {
            return await dbSet.AnyAsync(where);
        }

        #region 分页查询

        public virtual IEnumerable<T> LoadEntityEnumerable(Expression<Func<T, bool>> where, Expression<Func<T, string>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;

            if (asc.Equals(nameof(asc)))
                return (IEnumerable<T>)dbSet.Where<T>(where).OrderBy<T, string>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize);
            return (IEnumerable<T>)dbSet.Where<T>(where).OrderByDescending<T, string>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize);
        }
        public virtual IEnumerable<T> LoadEntityEnumerable(Expression<Func<T, bool>> where, Expression<Func<T, int?>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;

            if (asc.Equals(nameof(asc)))
                return (IEnumerable<T>)dbSet.Where<T>(where).OrderBy<T, int?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize);
            return (IEnumerable<T>)dbSet.Where<T>(where).OrderByDescending<T, int?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize);
        }
        public virtual IEnumerable<T> LoadEntityEnumerable(Expression<Func<T, bool>> where, Expression<Func<T, DateTime?>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;

            if (asc.Equals(nameof(asc)))
                return (IEnumerable<T>)dbSet.Where<T>(where).OrderBy<T, DateTime?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize);
            return (IEnumerable<T>)dbSet.Where<T>(where).OrderByDescending<T, DateTime?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize);
        }
        public virtual IEnumerable<T> LoadEntityEnumerable(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;

            if (asc.Equals(nameof(asc)))
                return (IEnumerable<T>)dbSet.Where<T>(where).OrderBy<T, Decimal?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize);
            return (IEnumerable<T>)dbSet.Where<T>(where).OrderByDescending<T, Decimal?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize);
        }
        public virtual IEnumerable<T> LoadEntityEnumerable(Expression<Func<T, bool>> where, Expression<Func<T, bool?>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;

            if (asc.Equals(nameof(asc)))
                return (IEnumerable<T>)dbSet.Where<T>(where).OrderBy<T, bool?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize);
            return (IEnumerable<T>)dbSet.Where<T>(where).OrderByDescending<T, bool?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize);
        }

        public virtual async Task<IEnumerable<T>> LoadEntityListAsync(Expression<Func<T, bool>> where, Expression<Func<T, string>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;

            if (asc.Equals(nameof(asc)))
                return await dbSet.Where<T>(where).OrderBy<T, string>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToListAsync();
            return await dbSet.Where<T>(where).OrderByDescending<T, string>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToListAsync();
        }
        public virtual async Task<IEnumerable<T>> LoadEntityListAsync(Expression<Func<T, bool>> where, Expression<Func<T, int?>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;

            if (asc.Equals(nameof(asc)))
                return await dbSet.Where<T>(where).OrderBy<T, int?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToListAsync();
            return await dbSet.Where<T>(where).OrderByDescending<T, int?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToListAsync();
        }
        public virtual async Task<IEnumerable<T>> LoadEntityListAsync(Expression<Func<T, bool>> where, Expression<Func<T, DateTime?>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;

            if (asc.Equals(nameof(asc)))
                return await dbSet.Where<T>(where).OrderBy<T, DateTime?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToListAsync();
            return await dbSet.Where<T>(where).OrderByDescending<T, DateTime?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToListAsync();
        }
        public virtual async Task<IEnumerable<T>> LoadEntityListAsync(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;

            if (asc.Equals(nameof(asc)))
                return await dbSet.Where<T>(where).OrderBy<T, Decimal?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToListAsync();
            return await dbSet.Where<T>(where).OrderByDescending<T, Decimal?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToListAsync();
        }

        #endregion

        #region 查询实体数量

        public virtual int GetEntityCount(Expression<Func<T, bool>> where)
        {
            return dbSet.Count<T>(where);
        }

        public virtual async Task<int> GetEntityCountAsync(Expression<Func<T, bool>> where)
        {
            return await dbSet.CountAsync<T>(where);
        }

        #endregion

        #region 求平均，求总计

        public virtual int? GetSum(Expression<Func<T, bool>> where, Expression<Func<T, int?>> sum)
        {
            return dbSet.Where<T>(where).Sum<T>(sum);
        }
        public virtual double? GetSum(Expression<Func<T, bool>> where, Expression<Func<T, double?>> sum)
        {
            return dbSet.Where<T>(where).Sum<T>(sum);
        }
        public virtual float? GetSum(Expression<Func<T, bool>> where, Expression<Func<T, float?>> sum)
        {
            return dbSet.Where<T>(where).Sum<T>(sum);
        }
        public virtual decimal? GetSum(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> sum)
        {
            return dbSet.Where<T>(where).Sum<T>(sum);
        }
        public virtual double? GetAvg(Expression<Func<T, bool>> where, Expression<Func<T, int?>> avg)
        {
            return dbSet.Where<T>(where).Average<T>(avg);
        }
        public virtual double? GetAvg(Expression<Func<T, bool>> where, Expression<Func<T, double?>> avg)
        {
            return dbSet.Where<T>(where).Average<T>(avg);
        }
        public virtual float? GetAvg(Expression<Func<T, bool>> where, Expression<Func<T, float?>> avg)
        {
            return dbSet.Where<T>(where).Average<T>(avg);
        }
        public virtual decimal? GetAvg(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> avg)
        {
            return dbSet.Where<T>(where).Average<T>(avg);
        }

        public virtual async Task<int?> GetSumAsync(Expression<Func<T, bool>> where, Expression<Func<T, int?>> sum)
        {
            return await dbSet.Where<T>(where).SumAsync<T>(sum);
        }
        public virtual async Task<double?> GetSumAsync(Expression<Func<T, bool>> where, Expression<Func<T, double?>> sum)
        {
            return await dbSet.Where<T>(where).SumAsync<T>(sum);
        }
        public virtual async Task<float?> GetSumAsync(Expression<Func<T, bool>> where, Expression<Func<T, float?>> sum)
        {
            return await dbSet.Where<T>(where).SumAsync<T>(sum);
        }
        public virtual async Task<decimal?> GetSumAsync(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> sum)
        {
            return await dbSet.Where<T>(where).SumAsync<T>(sum);
        }
        public virtual async Task<double?> GetAvgAsync(Expression<Func<T, bool>> where, Expression<Func<T, int?>> avg)
        {
            return await dbSet.Where<T>(where).AverageAsync<T>(avg);
        }
        public virtual async Task<double?> GetAvgAsync(Expression<Func<T, bool>> where, Expression<Func<T, double?>> avg)
        {
            return await dbSet.Where<T>(where).AverageAsync<T>(avg);
        }
        public virtual async Task<float?> GetAvgAsync(Expression<Func<T, bool>> where, Expression<Func<T, float?>> avg)
        {
            return await dbSet.Where<T>(where).AverageAsync<T>(avg);
        }
        public virtual async Task<decimal?> GetAvgAsync(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> avg)
        {
            return await dbSet.Where<T>(where).AverageAsync<T>(avg);
        }

        #endregion

        #region 查最大

        public virtual int? GetMax(Expression<Func<T, int?>> max)
        {
            return dbSet.Max<T, int?>(max);
        }
        public virtual double? GetMax(Expression<Func<T, double?>> max)
        {
            return dbSet.Max<T, double?>(max);
        }
        public virtual decimal? GetMax(Expression<Func<T, decimal?>> max)
        {
            return dbSet.Max<T, decimal?>(max);
        }
        public virtual DateTime? GetMax(Expression<Func<T, DateTime?>> max)
        {
            return dbSet.Max<T, DateTime?>(max);
        }

        public virtual async Task<int?> GetMaxAsync(Expression<Func<T, int?>> max)
        {
            return await dbSet.MaxAsync<T, int?>(max);
        }
        public virtual async Task<double?> GetMaxAsync(Expression<Func<T, double?>> max)
        {
            return await dbSet.MaxAsync<T, double?>(max);
        }
        public virtual async Task<decimal?> GetMaxAsync(Expression<Func<T, decimal?>> max)
        {
            return await dbSet.MaxAsync<T, decimal?>(max);
        }
        public virtual async Task<DateTime?> GetMaxAsync(Expression<Func<T, DateTime?>> max)
        {
            return await dbSet.MaxAsync<T, DateTime?>(max);
        }

        #endregion

        #endregion

        #region SQL执行

        public virtual IEnumerable<T> SqlQuery(string sql, params object[] parameters)
        {
            return Context.Database.SqlQuery<T>(sql, parameters);
        }
        public virtual int ExecuteSqlCommand(string sql, params object[] parameters)
        {
            return Context.Database.ExecuteSqlCommand(sql, parameters);
        }
        public virtual async Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters)
        {
            return await Context.Database.ExecuteSqlCommandAsync(sql, parameters);
        }

        #endregion
    }
}
