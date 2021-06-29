using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Z.EntityFramework.Plus;
using SysEntityFrameworkCore;
using System.Data.Common;

namespace SysRepository
{
    public class BaseRepository<T> : UnitOfWork, IBaseRepository<T> where T : class, new()
    {
        private readonly SysDbContext _dbContext;
        public BaseRepository(SysDbContext myContext) : base(myContext)
        {
            _dbContext = myContext;
        }

        #region 固定公用帮助，含事务

        /// <summary>
        /// 数据上下文
        /// </summary>
        public virtual DbContext Context
        {
            get
            {
                this.Context.Database.SetCommandTimeout(180);
                return _dbContext;
            }
        }
        /// <summary>
        /// 公用泛型处理属性
        /// 注:所有泛型操作的基础
        /// </summary>
        public virtual DbSet<T> DbSet
        {
            get { return Context.Set<T>(); }
        }
        /// <summary>
        /// 事务
        /// </summary>
        private DbTransaction _transaction = null;
        /// <summary>
        /// 开始事务
        /// </summary>
        public virtual DbTransaction Transaction
        {
            get
            {
                if (_transaction == null)
                {
                    _transaction = Context.Database.CurrentTransaction?.GetDbTransaction();
                }
                return _transaction;
            }
            set { _transaction = value; }
        }
        /// <summary>
        /// 事务状态
        /// </summary>
        public virtual bool Committed { get; set; }
        /// <summary>
        /// 异步锁定
        /// </summary>
        private readonly object sync = new object();
        /// <summary>
        /// 提交事务
        /// </summary>
        public virtual void CommitAffair()
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
        public virtual void RollbackAffair()
        {
            Committed = false;
            if (_transaction != null)
            {
                _transaction.Rollback();
            }
        }

        #endregion

        #region  新增

        public virtual void Insert(T entity)
        {
            _dbContext.Set<T>().Add(entity);
        }
        public virtual void Insert(IEnumerable<T> entity)
        {
            _dbContext.Set<T>().AddRange(entity);
        }

        public virtual async Task InsertAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
        }
        public virtual async Task InsertAsync(IEnumerable<T> entity)
        {
            await _dbContext.Set<T>().AddRangeAsync(entity);
        }

        #endregion

        #region 修改

        public virtual void Update(T entity)
        {
            _dbContext.Entry<T>(entity).State = EntityState.Modified;
        }
        public virtual int Update(Expression<Func<T, bool>> where, Expression<Func<T, T>> entity)
        {
            return _dbContext.Set<T>().Where(where).Update(entity);
        }
        public virtual async Task<int> UpdateAsync(Expression<Func<T, bool>> where, Expression<Func<T, T>> entity)
        {
            return await _dbContext.Set<T>().Where(where).UpdateAsync(entity);
        }

        #endregion

        #region 删除

        public virtual int Delete(Expression<Func<T, bool>> where)
        {
            return _dbContext.Set<T>().Where(where).Delete();
        }
        public virtual async Task<int> DeleteAsync(Expression<Func<T, bool>> where)
        {
            return await _dbContext.Set<T>().Where(where).DeleteAsync();
        }

        #endregion

        #region 查询

        public virtual T Find(object id)
        {
            return _dbContext.Set<T>().Find(id);
        }
        public virtual T Get(Expression<Func<T, bool>> where)
        {
            return _dbContext.Set<T>().FirstOrDefault(where);
        }
        public virtual List<T> GetAll()
        {
            return _dbContext.Set<T>().ToList();
        }
        public virtual List<T> GetAll(Expression<Func<T, bool>> where)
        {
            return _dbContext.Set<T>().Where(where).ToList();
        }
        public virtual bool Any(Expression<Func<T, bool>> where)
        {
            return _dbContext.Set<T>().Any(where);
        }

        public virtual async Task<T> FindAsync(object id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }
        public virtual async Task<T> GetAsync(Expression<Func<T, bool>> where)
        {
            return await _dbContext.Set<T>().Where(where).FirstOrDefaultAsync();
        }
        public virtual async Task<List<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }
        public virtual async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> where)
        {
            return await _dbContext.Set<T>().Where(where).ToListAsync();
        }
        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> where)
        {
            return await _dbContext.Set<T>().AnyAsync(where);
        }

        #region 分页

        public virtual IEnumerable<T> PagedQuery(Expression<Func<T, bool>> where, Expression<Func<T, string>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;

            if (asc.Equals(nameof(asc)))
                return (IEnumerable<T>)_dbContext.Set<T>().Where<T>(where).OrderBy<T, string>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize);
            return (IEnumerable<T>)_dbContext.Set<T>().Where<T>(where).OrderByDescending<T, string>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize);
        }
        public virtual IEnumerable<T> PagedQuery(Expression<Func<T, bool>> where, Expression<Func<T, int?>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;

            if (asc.Equals(nameof(asc)))
                return (IEnumerable<T>)_dbContext.Set<T>().Where<T>(where).OrderBy<T, int?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize);
            return (IEnumerable<T>)_dbContext.Set<T>().Where<T>(where).OrderByDescending<T, int?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize);

        }
        public virtual IEnumerable<T> PagedQuery(Expression<Func<T, bool>> where, Expression<Func<T, DateTime?>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;

            if (asc.Equals(nameof(asc)))
                return (IEnumerable<T>)_dbContext.Set<T>().Where<T>(where).OrderBy<T, DateTime?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize);
            return (IEnumerable<T>)_dbContext.Set<T>().Where<T>(where).OrderByDescending<T, DateTime?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize);

        }
        public virtual IEnumerable<T> PagedQuery(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;

            if (asc.Equals(nameof(asc)))
                return (IEnumerable<T>)_dbContext.Set<T>().Where<T>(where).OrderBy<T, Decimal?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize);
            return (IEnumerable<T>)_dbContext.Set<T>().Where<T>(where).OrderByDescending<T, Decimal?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize);

        }
        public virtual IEnumerable<T> PagedQuery(Expression<Func<T, bool>> where, Expression<Func<T, bool?>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;

            if (asc.Equals(nameof(asc)))
                return (IEnumerable<T>)_dbContext.Set<T>().Where<T>(where).OrderBy<T, bool?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize);
            return (IEnumerable<T>)_dbContext.Set<T>().Where<T>(where).OrderByDescending<T, bool?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize);

        }

        public virtual async Task<List<T>> PagedQueryAsync(Expression<Func<T, bool>> where, Expression<Func<T, string>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;

            if (asc.Equals(nameof(asc)))
                return await _dbContext.Set<T>().Where<T>(where).OrderBy<T, string>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToListAsync();
            return await _dbContext.Set<T>().Where<T>(where).OrderByDescending<T, string>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToListAsync();

        }
        public virtual async Task<List<T>> PagedQueryAsync(Expression<Func<T, bool>> where, Expression<Func<T, int?>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;

            if (asc.Equals(nameof(asc)))
                return await _dbContext.Set<T>().Where<T>(where).OrderBy<T, int?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToListAsync();
            return await _dbContext.Set<T>().Where<T>(where).OrderByDescending<T, int?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToListAsync();

        }
        public virtual async Task<List<T>> PagedQueryAsync(Expression<Func<T, bool>> where, Expression<Func<T, DateTime?>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;

            if (asc.Equals(nameof(asc)))
                return await _dbContext.Set<T>().Where<T>(where).OrderBy<T, DateTime?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToListAsync();
            return await _dbContext.Set<T>().Where<T>(where).OrderByDescending<T, DateTime?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToListAsync();

        }
        public virtual async Task<List<T>> PagedQueryAsync(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;

            if (asc.Equals(nameof(asc)))
                return await _dbContext.Set<T>().Where<T>(where).OrderBy<T, Decimal?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToListAsync();
            return await _dbContext.Set<T>().Where<T>(where).OrderByDescending<T, Decimal?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToListAsync();

        }

        #endregion

        #region 查询实体数量

        public virtual int Count(Expression<Func<T, bool>> where)
        {
            return _dbContext.Set<T>().Count<T>(where);
        }
        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> where)
        {
            return await _dbContext.Set<T>().CountAsync<T>(where);
        }

        #endregion

        #region 求平均，求总计

        public virtual int? Sum(Expression<Func<T, bool>> where, Expression<Func<T, int?>> sum)
        {
            return _dbContext.Set<T>().Where<T>(where).Sum<T>(sum);
        }
        public virtual double? Sum(Expression<Func<T, bool>> where, Expression<Func<T, double?>> sum)
        {
            return _dbContext.Set<T>().Where<T>(where).Sum<T>(sum);
        }
        public virtual float? Sum(Expression<Func<T, bool>> where, Expression<Func<T, float?>> sum)
        {
            return _dbContext.Set<T>().Where<T>(where).Sum<T>(sum);
        }
        public virtual decimal? Sum(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> sum)
        {
            return _dbContext.Set<T>().Where<T>(where).Sum<T>(sum);
        }
        public virtual double? Avg(Expression<Func<T, bool>> where, Expression<Func<T, int?>> avg)
        {
            return _dbContext.Set<T>().Where<T>(where).Average<T>(avg);
        }
        public virtual double? Avg(Expression<Func<T, bool>> where, Expression<Func<T, double?>> avg)
        {
            return _dbContext.Set<T>().Where<T>(where).Average<T>(avg);
        }
        public virtual float? Avg(Expression<Func<T, bool>> where, Expression<Func<T, float?>> avg)
        {
            return _dbContext.Set<T>().Where<T>(where).Average<T>(avg);
        }
        public virtual decimal? Avg(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> avg)
        {
            return _dbContext.Set<T>().Where<T>(where).Average<T>(avg);
        }

        public virtual async Task<int?> SumAsync(Expression<Func<T, bool>> where, Expression<Func<T, int?>> sum)
        {
            return await _dbContext.Set<T>().Where<T>(where).SumAsync<T>(sum);
        }
        public virtual async Task<double?> SumAsync(Expression<Func<T, bool>> where, Expression<Func<T, double?>> sum)
        {
            return await _dbContext.Set<T>().Where<T>(where).SumAsync<T>(sum);
        }
        public virtual async Task<float?> SumAsync(Expression<Func<T, bool>> where, Expression<Func<T, float?>> sum)
        {
            return await _dbContext.Set<T>().Where<T>(where).SumAsync<T>(sum);
        }
        public virtual async Task<decimal?> SumAsync(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> sum)
        {
            return await _dbContext.Set<T>().Where<T>(where).SumAsync<T>(sum);
        }
        public virtual async Task<double?> AvgAsync(Expression<Func<T, bool>> where, Expression<Func<T, int?>> avg)
        {
            return await _dbContext.Set<T>().Where<T>(where).AverageAsync<T>(avg);
        }
        public virtual async Task<double?> AvgAsync(Expression<Func<T, bool>> where, Expression<Func<T, double?>> avg)
        {
            return await _dbContext.Set<T>().Where<T>(where).AverageAsync<T>(avg);
        }
        public virtual async Task<float?> AvgAsync(Expression<Func<T, bool>> where, Expression<Func<T, float?>> avg)
        {
            return await _dbContext.Set<T>().Where<T>(where).AverageAsync<T>(avg);
        }
        public virtual async Task<decimal?> AvgAsync(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> avg)
        {
            return await _dbContext.Set<T>().Where<T>(where).AverageAsync<T>(avg);
        }

        #endregion

        #region 查最大

        public virtual int? Max(Expression<Func<T, int?>> max)
        {
            return _dbContext.Set<T>().Max<T, int?>(max);
        }
        public virtual double? Max(Expression<Func<T, double?>> max)
        {
            return _dbContext.Set<T>().Max<T, double?>(max);
        }
        public virtual decimal? Max(Expression<Func<T, decimal?>> max)
        {
            return _dbContext.Set<T>().Max<T, decimal?>(max);
        }
        public virtual DateTime? Max(Expression<Func<T, DateTime?>> max)
        {
            return _dbContext.Set<T>().Max<T, DateTime?>(max);
        }

        public virtual async Task<int?> MaxAsync(Expression<Func<T, int?>> max)
        {
            return await _dbContext.Set<T>().MaxAsync<T, int?>(max);
        }
        public virtual async Task<double?> MaxAsync(Expression<Func<T, double?>> max)
        {
            return await _dbContext.Set<T>().MaxAsync<T, double?>(max);
        }
        public virtual async Task<decimal?> MaxAsync(Expression<Func<T, decimal?>> max)
        {
            return await _dbContext.Set<T>().MaxAsync<T, decimal?>(max);
        }
        public virtual async Task<DateTime?> MaxAsync(Expression<Func<T, DateTime?>> max)
        {
            return await _dbContext.Set<T>().MaxAsync<T, DateTime?>(max);
        }

        #endregion

        #endregion

        #region SQL执行

        public virtual void ExecuteSql(string sql, params object[] parameters)
        {
            _dbContext.Database.ExecuteSqlRaw(sql, parameters);
        }
        public virtual List<T> QuerySql(string sql, params object[] parameters)
        {
            return _dbContext.Set<T>().FromSqlRaw<T>(sql, parameters).ToList();
        }

        public virtual async Task ExecuteSqlAsync(string sql, params object[] parameters)
        {
            await _dbContext.Database.ExecuteSqlRawAsync(sql, parameters);
        }
        public virtual async Task<List<T>> QuerySqlAsync(string sql, params object[] parameters)
        {
            return await _dbContext.Set<T>().FromSqlRaw<T>(sql, parameters).ToListAsync();
        }

        #endregion
    }
}
