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
        public DbSet<T> DbSet
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

        public virtual bool Insert(T entity, bool isCommit = true)
        {
            DbSet.Add(entity);
            return !isCommit || Context.SaveChanges() > 0;
        }
        public virtual bool Insert(IEnumerable<T> entitys, bool isCommit = true)
        {
            DbSet.AddRange(entitys);
            return !isCommit || Context.SaveChanges() > 0;
        }

        public virtual async Task<bool> InsertAsync(T entity, bool isCommit = true)
        {
            DbSet.Add(entity);
            if (!isCommit)
            {
                return false;
            }
            return await Context.SaveChangesAsync() > 0;
        }
        public virtual async Task<bool> InsertAsync(IEnumerable<T> entitys, bool isCommit = true)
        {
            DbSet.AddRange(entitys);
            if (!isCommit)
            {
                return false;
            }
            return await Context.SaveChangesAsync() > 0;
        }

        #endregion

        #region 修改

        public virtual bool Update(T entity, bool isCommit = true)
        {
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }
            Context.Entry(entity).State = EntityState.Modified;
            return !isCommit || Context.SaveChanges() > 0;
        }
        public virtual bool Update(IEnumerable<T> entitys, bool isCommit = true)
        {
            foreach (var entity in entitys)
            {
                if (Context.Entry(entity).State == EntityState.Detached)
                {
                    DbSet.Attach(entity);
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
                DbSet.Attach(entity);
            }
            Context.Entry(entity).State = EntityState.Modified;
            return await Context.SaveChangesAsync() > 0;
        }
        public virtual async Task<bool> UpdateAsync(IEnumerable<T> entities, bool isCommit = true)
        {
            if (!isCommit)
            {
                return false;
            }
            foreach (var entity in entities)
            {
                if (Context.Entry(entity).State == EntityState.Detached)
                {
                    DbSet.Attach(entity);
                }
                Context.Entry(entity).State = EntityState.Modified;
            }
            return await Context.SaveChangesAsync() > 0;
        }

        #endregion

        #region 删除

        public virtual bool Delete(T entity, bool isCommit = true)
        {
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }
            DbSet.Remove(entity);
            return !isCommit || Context.SaveChanges() > 0;
        }
        public virtual bool Delete(Expression<Func<T, bool>> filter, bool isCommit = true)
        {
            var entitys = DbSet.Where(filter);
            DbSet.RemoveRange(entitys);
            return !isCommit || Context.SaveChanges() > 0;
        }

        public virtual async Task<bool> DeleteAsync(T entity, bool isCommit = true)
        {
            if (!isCommit)
            {
                return false;
            }
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }
            DbSet.Remove(entity);
            return await Context.SaveChangesAsync() > 0;
        }
        public virtual async Task<bool> DeleteAsync(Expression<Func<T, bool>> filter, bool isCommit = true)
        {
            if (!isCommit)
            {
                return false;
            }
            var entitys = DbSet.Where(filter);
            DbSet.RemoveRange(entitys);
            return await Context.SaveChangesAsync() > 0;
        }

        #endregion

        #region 查询

        public virtual T Find(object id)
        {
            return DbSet.Find(id);
        }
        public virtual IEnumerable<T> GetAll()
        {
            return DbSet.ToList();
        }
        public virtual IEnumerable<T> GetAll(Expression<Func<T, bool>> where)
        {
            return DbSet.Where(where).ToList();
        }
        public virtual T Get(Expression<Func<T, bool>> where)
        {
            return DbSet.Where(where).FirstOrDefault();
        }
        public virtual bool Any(Expression<Func<T, bool>> where)
        {
            return DbSet.Any(where);
        }

        public virtual async Task<T> FindAsync(object id)
        {
            return await DbSet.FindAsync(id);
        }
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await DbSet.ToListAsync();
        }
        public virtual async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> where)
        {
            return await DbSet.Where(where).ToListAsync();
        }
        public virtual async Task<T> GetAsync(Expression<Func<T, bool>> where)
        {
            return await DbSet.Where(where).FirstOrDefaultAsync();
        }
        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> where)
        {
            return await DbSet.AnyAsync(where);
        }

        #region 分页查询

        public virtual IEnumerable<T> PagedQuery(Expression<Func<T, bool>> where, Expression<Func<T, string>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;

            if (asc.Equals(nameof(asc)))
                return (IEnumerable<T>)DbSet.Where<T>(where).OrderBy<T, string>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize);
            return (IEnumerable<T>)DbSet.Where<T>(where).OrderByDescending<T, string>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize);
        }
        public virtual IEnumerable<T> PagedQuery(Expression<Func<T, bool>> where, Expression<Func<T, int?>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;

            if (asc.Equals(nameof(asc)))
                return (IEnumerable<T>)DbSet.Where<T>(where).OrderBy<T, int?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize);
            return (IEnumerable<T>)DbSet.Where<T>(where).OrderByDescending<T, int?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize);
        }
        public virtual IEnumerable<T> PagedQuery(Expression<Func<T, bool>> where, Expression<Func<T, DateTime?>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;

            if (asc.Equals(nameof(asc)))
                return (IEnumerable<T>)DbSet.Where<T>(where).OrderBy<T, DateTime?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize);
            return (IEnumerable<T>)DbSet.Where<T>(where).OrderByDescending<T, DateTime?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize);
        }
        public virtual IEnumerable<T> PagedQuery(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;

            if (asc.Equals(nameof(asc)))
                return (IEnumerable<T>)DbSet.Where<T>(where).OrderBy<T, Decimal?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize);
            return (IEnumerable<T>)DbSet.Where<T>(where).OrderByDescending<T, Decimal?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize);
        }
        public virtual IEnumerable<T> PagedQuery(Expression<Func<T, bool>> where, Expression<Func<T, bool?>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;

            if (asc.Equals(nameof(asc)))
                return (IEnumerable<T>)DbSet.Where<T>(where).OrderBy<T, bool?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize);
            return (IEnumerable<T>)DbSet.Where<T>(where).OrderByDescending<T, bool?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize);
        }

        public virtual async Task<IEnumerable<T>> PagedQueryAsync(Expression<Func<T, bool>> where, Expression<Func<T, string>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;

            if (asc.Equals(nameof(asc)))
                return await DbSet.Where<T>(where).OrderBy<T, string>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToListAsync();
            return await DbSet.Where<T>(where).OrderByDescending<T, string>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToListAsync();
        }
        public virtual async Task<IEnumerable<T>> PagedQueryAsync(Expression<Func<T, bool>> where, Expression<Func<T, int?>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;

            if (asc.Equals(nameof(asc)))
                return await DbSet.Where<T>(where).OrderBy<T, int?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToListAsync();
            return await DbSet.Where<T>(where).OrderByDescending<T, int?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToListAsync();
        }
        public virtual async Task<IEnumerable<T>> PagedQueryAsync(Expression<Func<T, bool>> where, Expression<Func<T, DateTime?>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;

            if (asc.Equals(nameof(asc)))
                return await DbSet.Where<T>(where).OrderBy<T, DateTime?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToListAsync();
            return await DbSet.Where<T>(where).OrderByDescending<T, DateTime?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToListAsync();
        }
        public virtual async Task<IEnumerable<T>> PagedQueryAsync(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;

            if (asc.Equals(nameof(asc)))
                return await DbSet.Where<T>(where).OrderBy<T, Decimal?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToListAsync();
            return await DbSet.Where<T>(where).OrderByDescending<T, Decimal?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToListAsync();
        }

        #endregion

        #region 查询实体数量

        public virtual int Count(Expression<Func<T, bool>> where)
        {
            return DbSet.Count<T>(where);
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> where)
        {
            return await DbSet.CountAsync<T>(where);
        }

        #endregion

        #region 求平均，求总计

        public virtual int? Sum(Expression<Func<T, bool>> where, Expression<Func<T, int?>> sum)
        {
            return DbSet.Where<T>(where).Sum<T>(sum);
        }
        public virtual double? Sum(Expression<Func<T, bool>> where, Expression<Func<T, double?>> sum)
        {
            return DbSet.Where<T>(where).Sum<T>(sum);
        }
        public virtual float? Sum(Expression<Func<T, bool>> where, Expression<Func<T, float?>> sum)
        {
            return DbSet.Where<T>(where).Sum<T>(sum);
        }
        public virtual decimal? Sum(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> sum)
        {
            return DbSet.Where<T>(where).Sum<T>(sum);
        }
        public virtual double? Avg(Expression<Func<T, bool>> where, Expression<Func<T, int?>> avg)
        {
            return DbSet.Where<T>(where).Average<T>(avg);
        }
        public virtual double? Avg(Expression<Func<T, bool>> where, Expression<Func<T, double?>> avg)
        {
            return DbSet.Where<T>(where).Average<T>(avg);
        }
        public virtual float? Avg(Expression<Func<T, bool>> where, Expression<Func<T, float?>> avg)
        {
            return DbSet.Where<T>(where).Average<T>(avg);
        }
        public virtual decimal? Avg(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> avg)
        {
            return DbSet.Where<T>(where).Average<T>(avg);
        }

        public virtual async Task<int?> SumAsync(Expression<Func<T, bool>> where, Expression<Func<T, int?>> sum)
        {
            return await DbSet.Where<T>(where).SumAsync<T>(sum);
        }
        public virtual async Task<double?> SumAsync(Expression<Func<T, bool>> where, Expression<Func<T, double?>> sum)
        {
            return await DbSet.Where<T>(where).SumAsync<T>(sum);
        }
        public virtual async Task<float?> SumAsync(Expression<Func<T, bool>> where, Expression<Func<T, float?>> sum)
        {
            return await DbSet.Where<T>(where).SumAsync<T>(sum);
        }
        public virtual async Task<decimal?> SumAsync(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> sum)
        {
            return await DbSet.Where<T>(where).SumAsync<T>(sum);
        }
        public virtual async Task<double?> AvgAsync(Expression<Func<T, bool>> where, Expression<Func<T, int?>> avg)
        {
            return await DbSet.Where<T>(where).AverageAsync<T>(avg);
        }
        public virtual async Task<double?> AvgAsync(Expression<Func<T, bool>> where, Expression<Func<T, double?>> avg)
        {
            return await DbSet.Where<T>(where).AverageAsync<T>(avg);
        }
        public virtual async Task<float?> AvgAsync(Expression<Func<T, bool>> where, Expression<Func<T, float?>> avg)
        {
            return await DbSet.Where<T>(where).AverageAsync<T>(avg);
        }
        public virtual async Task<decimal?> AvgAsync(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> avg)
        {
            return await DbSet.Where<T>(where).AverageAsync<T>(avg);
        }

        #endregion

        #region 查最大

        public virtual int? Max(Expression<Func<T, int?>> max)
        {
            return DbSet.Max<T, int?>(max);
        }
        public virtual double? Max(Expression<Func<T, double?>> max)
        {
            return DbSet.Max<T, double?>(max);
        }
        public virtual decimal? Max(Expression<Func<T, decimal?>> max)
        {
            return DbSet.Max<T, decimal?>(max);
        }
        public virtual DateTime? Max(Expression<Func<T, DateTime?>> max)
        {
            return DbSet.Max<T, DateTime?>(max);
        }

        public virtual async Task<int?> MaxAsync(Expression<Func<T, int?>> max)
        {
            return await DbSet.MaxAsync<T, int?>(max);
        }
        public virtual async Task<double?> MaxAsync(Expression<Func<T, double?>> max)
        {
            return await DbSet.MaxAsync<T, double?>(max);
        }
        public virtual async Task<decimal?> MaxAsync(Expression<Func<T, decimal?>> max)
        {
            return await DbSet.MaxAsync<T, decimal?>(max);
        }
        public virtual async Task<DateTime?> MaxAsync(Expression<Func<T, DateTime?>> max)
        {
            return await DbSet.MaxAsync<T, DateTime?>(max);
        }

        #endregion

        #endregion

        #region SQL执行

        public virtual IEnumerable<T> SqlQuery(string sql, params object[] parameters)
        {
            return Context.Database.SqlQuery<T>(sql, parameters);
        }
        public virtual int ExecuteSql(string sql, params object[] parameters)
        {
            return Context.Database.ExecuteSqlCommand(sql, parameters);
        }
        public virtual async Task<int> ExecuteSqlAsync(string sql, params object[] parameters)
        {
            return await Context.Database.ExecuteSqlCommandAsync(sql, parameters);
        }

        #endregion
    }
}
