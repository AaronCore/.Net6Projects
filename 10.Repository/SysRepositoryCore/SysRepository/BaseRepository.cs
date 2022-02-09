using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;
using SysEntityFrameworkCore;

namespace SysRepository
{
    public class BaseRepository<T> : UnitOfWork, IBaseRepository<T> where T : class, new()
    {
        private readonly SysDbContext _dbContext;
        public BaseRepository(SysDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        #region 同步

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual void Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
        }

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual void AddRange(List<T> entity)
        {
            _dbContext.Set<T>().AddRange(entity);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual int Delete(Expression<Func<T, bool>> where)
        {
            return _dbContext.Set<T>().Where(where).Delete();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual void Update(T entity)
        {
            _dbContext.Entry<T>(entity).State = EntityState.Modified;
        }

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="where"></param>
        /// <param name="entity"></param>
        public virtual int UpdateRange(Expression<Func<T, bool>> where, Expression<Func<T, T>> entity)
        {
            return _dbContext.Set<T>().Where(where).Update(entity);
        }

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetAll()
        {
            return _dbContext.Set<T>().ToList();
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public IEnumerable<T> Get(Expression<Func<T, bool>> where)
        {
            return _dbContext.Set<T>().Where(where).ToList();
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public T? GetEntity(Expression<Func<T, bool>> where)
        {
            return _dbContext.Set<T>().FirstOrDefault(where);
        }

        /// <summary>
        /// 根据主键获取
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T? Find(object? key)
        {
            return _dbContext.Set<T>().Find(key);
        }

        /// <summary>
        /// 获取最大的一条数据
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public T? GetMaxSort(Expression<Func<T, int>> order)
        {
            return _dbContext.Set<T>().OrderByDescending(order).FirstOrDefault();
        }

        /// <summary>
        /// 获取最小的一条数据
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public T? GetMinSort(Expression<Func<T, int>> order)
        {
            return _dbContext.Set<T>().OrderBy(order).FirstOrDefault();
        }

        /// <summary>
        /// Any 查找数据判断数据是否存在
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public bool Any(Expression<Func<T, bool>> where)
        {
            return _dbContext.Set<T>().Any(where);
        }

        /// <summary>
        /// 查询数量
        /// </summary>
        /// <returns></returns>
        public virtual int GetCount()
        {
            return _dbContext.Set<T>().Count<T>();
        }

        /// <summary>
        /// 查询数量
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual int GetCount(Expression<Func<T, bool>> where)
        {
            return _dbContext.Set<T>().Count<T>(where);
        }

        #region 分页

        public virtual List<T> Load(Expression<Func<T, bool>> where, Expression<Func<T, string>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;
            if (asc.Equals(nameof(asc)))
            {
                return _dbContext.Set<T>().Where<T>(where).OrderBy<T, string>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToList();
            }
            return _dbContext.Set<T>().Where<T>(where).OrderByDescending<T, string>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToList();
        }
        public virtual List<T> Load(Expression<Func<T, bool>> where, Expression<Func<T, int?>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;
            if (asc.Equals(nameof(asc)))
            {
                return _dbContext.Set<T>().Where<T>(where).OrderBy<T, int?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToList();
            }
            return _dbContext.Set<T>().Where<T>(where).OrderByDescending<T, int?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToList();
        }
        public virtual List<T> Load(Expression<Func<T, bool>> where, Expression<Func<T, DateTime?>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;
            if (asc.Equals(nameof(asc)))
            {
                return _dbContext.Set<T>().Where<T>(where).OrderBy<T, DateTime?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToList();
            }
            return _dbContext.Set<T>().Where<T>(where).OrderByDescending<T, DateTime?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToList();
        }
        public virtual List<T> Load(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;
            if (asc.Equals(nameof(asc)))
            {
                return _dbContext.Set<T>().Where<T>(where).OrderBy<T, Decimal?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToList();
            }
            return _dbContext.Set<T>().Where<T>(where).OrderByDescending<T, Decimal?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToList();
        }
        public virtual List<T> Load(Expression<Func<T, bool>> where, Expression<Func<T, bool?>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;
            if (asc.Equals(nameof(asc)))
            {
                return _dbContext.Set<T>().Where<T>(where).OrderBy<T, bool?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToList();
            }
            return _dbContext.Set<T>().Where<T>(where).OrderByDescending<T, bool?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToList();
        }

        #endregion

        #region 求总计

        public virtual int? GetSum(Expression<Func<T, bool>> where, Expression<Func<T, int?>> sum)
        {
            return _dbContext.Set<T>().Where<T>(where).Sum<T>(sum);
        }
        public virtual double? GetSum(Expression<Func<T, bool>> where, Expression<Func<T, double?>> sum)
        {
            return _dbContext.Set<T>().Where<T>(where).Sum<T>(sum);
        }
        public virtual float? GetSum(Expression<Func<T, bool>> where, Expression<Func<T, float?>> sum)
        {
            return _dbContext.Set<T>().Where<T>(where).Sum<T>(sum);
        }
        public virtual decimal? GetSum(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> sum)
        {
            return _dbContext.Set<T>().Where<T>(where).Sum<T>(sum);
        }

        #endregion

        #region 求平均

        public virtual double? GetAvg(Expression<Func<T, bool>> where, Expression<Func<T, int?>> avg)
        {
            return _dbContext.Set<T>().Where<T>(where).Average<T>(avg);
        }
        public virtual double? GetAvg(Expression<Func<T, bool>> where, Expression<Func<T, double?>> avg)
        {
            return _dbContext.Set<T>().Where<T>(where).Average<T>(avg);
        }
        public virtual float? GetAvg(Expression<Func<T, bool>> where, Expression<Func<T, float?>> avg)
        {
            return _dbContext.Set<T>().Where<T>(where).Average<T>(avg);
        }
        public virtual decimal? GetAvg(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> avg)
        {
            return _dbContext.Set<T>().Where<T>(where).Average<T>(avg);
        }

        #endregion

        #region 查最大

        public virtual int? GetMax(Expression<Func<T, int?>> max)
        {
            return _dbContext.Set<T>().Max<T, int?>(max);
        }
        public virtual double? GetMax(Expression<Func<T, double?>> max)
        {
            return _dbContext.Set<T>().Max<T, double?>(max);
        }
        public virtual decimal? GetMax(Expression<Func<T, decimal?>> max)
        {
            return _dbContext.Set<T>().Max<T, decimal?>(max);
        }
        public virtual DateTime? GetMax(Expression<Func<T, DateTime?>> max)
        {
            return _dbContext.Set<T>().Max<T, DateTime?>(max);
        }

        #endregion

        #region 查最小

        public virtual int? GetMin(Expression<Func<T, int?>> min)
        {
            return _dbContext.Set<T>().Min<T, int?>(min);
        }
        public virtual double? GetMin(Expression<Func<T, double?>> min)
        {
            return _dbContext.Set<T>().Min<T, double?>(min);
        }
        public virtual decimal? GetMin(Expression<Func<T, decimal?>> min)
        {
            return _dbContext.Set<T>().Min<T, decimal?>(min);
        }
        public virtual DateTime? GetMin(Expression<Func<T, DateTime?>> min)
        {
            return _dbContext.Set<T>().Min<T, DateTime?>(min);
        }

        #endregion

        #region SQL执行

        /// <summary>
        /// 执行sql语句
        /// </summary>
        public virtual void Execute(string sql)
        {
            _dbContext.Database.ExecuteSqlRaw(sql);
        }
        public virtual void Execute(string sql, object parameters)
        {
            _dbContext.Database.ExecuteSqlRaw(sql, parameters);
        }

        /// <summary>
        /// 使用sql脚本查询实体列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual List<T> Query(string sql)
        {
            return _dbContext.Set<T>().FromSqlRaw<T>(sql).ToList();
        }
        public virtual List<T> Query(string sql, object parameters)
        {
            return _dbContext.Set<T>().FromSqlRaw<T>(sql, parameters).ToList();
        }

        #endregion

        #endregion

        #region 异步

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
        }

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task AddRangeAsync(List<T> entity)
        {
            await _dbContext.Set<T>().AddRangeAsync(entity);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="where">实体列表</param>
        /// <returns></returns>
        public virtual async Task<int> DeleteAsync(Expression<Func<T, bool>> where)
        {
            return await _dbContext.Set<T>().Where(where).DeleteAsync();
        }

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="where"></param>
        /// <param name="entity"></param>
        public virtual async Task<int> UpdateRangeAsync(Expression<Func<T, bool>> where, Expression<Func<T, T>> entity)
        {
            return await _dbContext.Set<T>().Where(where).UpdateAsync(entity);
        }

        /// <summary>
        /// 异步查询所有
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> where)
        {
            return await _dbContext.Set<T>().Where(where).ToListAsync();
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public async Task<T?> GetEntityAsync(Expression<Func<T, bool>> where)
        {
            return await _dbContext.Set<T>().FirstOrDefaultAsync(where);
        }

        /// <summary>
        /// 根据主键获取
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T?> FindAsync(object? key)
        {
            return await _dbContext.Set<T>().FindAsync(key);
        }

        /// <summary>
        /// 获取最大的一条数据
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public async Task<T?> GetMaxSortAsync(Expression<Func<T, int>> order)
        {
            return await _dbContext.Set<T>().OrderByDescending(order).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 获取最小的一条数据
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public async Task<T?> GetMinSortAsync(Expression<Func<T, int>> order)
        {
            return await _dbContext.Set<T>().OrderBy(order).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Any 查找数据判断数据是否存在
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> where)
        {
            return await _dbContext.Set<T>().AnyAsync(where);
        }

        /// <summary>
        /// 查询数量
        /// </summary>
        /// <returns></returns>
        public virtual async Task<int> GetCountAsync()
        {
            return await _dbContext.Set<T>().CountAsync<T>();
        }

        /// <summary>
        /// 查询数量
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual async Task<int> GetCountAsync(Expression<Func<T, bool>> where)
        {
            return await _dbContext.Set<T>().CountAsync<T>(where);
        }

        #region 分页

        public virtual async Task<List<T>> LoadAsync(Expression<Func<T, bool>> where, Expression<Func<T, string>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;
            if (asc.Equals(nameof(asc)))
            {
                return await _dbContext.Set<T>().Where<T>(where).OrderBy<T, string>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToListAsync();
            }
            return await _dbContext.Set<T>().Where<T>(where).OrderByDescending<T, string>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToListAsync();
        }
        public virtual async Task<List<T>> LoadAsync(Expression<Func<T, bool>> where, Expression<Func<T, int?>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;
            if (asc.Equals(nameof(asc)))
            {
                return await _dbContext.Set<T>().Where<T>(where).OrderBy<T, int?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToListAsync();
            }
            return await _dbContext.Set<T>().Where<T>(where).OrderByDescending<T, int?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToListAsync();

        }
        public virtual async Task<List<T>> LoadAsync(Expression<Func<T, bool>> where, Expression<Func<T, DateTime?>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;
            if (asc.Equals(nameof(asc)))
            {
                return await _dbContext.Set<T>().Where<T>(where).OrderBy<T, DateTime?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToListAsync();
            }
            return await _dbContext.Set<T>().Where<T>(where).OrderByDescending<T, DateTime?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToListAsync();

        }
        public virtual async Task<List<T>> LoadAsync(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> orderby, string asc, int pageIndex, int pageSize)
        {
            --pageIndex;
            if (asc.Equals(nameof(asc)))
            {
                return await _dbContext.Set<T>().Where<T>(where).OrderBy<T, Decimal?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToListAsync();
            }
            return await _dbContext.Set<T>().Where<T>(where).OrderByDescending<T, Decimal?>(orderby).Skip<T>(pageIndex * pageSize).Take<T>(pageSize).ToListAsync();
        }

        #endregion

        #region 求总计

        public virtual async Task<int?> GetSumAsync(Expression<Func<T, bool>> where, Expression<Func<T, int?>> sum)
        {
            return await _dbContext.Set<T>().Where<T>(where).SumAsync<T>(sum);
        }
        public virtual async Task<double?> GetSumAsync(Expression<Func<T, bool>> where, Expression<Func<T, double?>> sum)
        {
            return await _dbContext.Set<T>().Where<T>(where).SumAsync<T>(sum);
        }
        public virtual async Task<float?> GetSumAsync(Expression<Func<T, bool>> where, Expression<Func<T, float?>> sum)
        {
            return await _dbContext.Set<T>().Where<T>(where).SumAsync<T>(sum);
        }
        public virtual async Task<decimal?> GetSumAsync(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> sum)
        {
            return await _dbContext.Set<T>().Where<T>(where).SumAsync<T>(sum);
        }

        #endregion

        #region 求平均

        public virtual async Task<double?> GetAvgAsync(Expression<Func<T, bool>> where, Expression<Func<T, int?>> avg)
        {
            return await _dbContext.Set<T>().Where<T>(where).AverageAsync<T>(avg);
        }
        public virtual async Task<double?> GetAvgAsync(Expression<Func<T, bool>> where, Expression<Func<T, double?>> avg)
        {
            return await _dbContext.Set<T>().Where<T>(where).AverageAsync<T>(avg);
        }
        public virtual async Task<float?> GetAvgAsync(Expression<Func<T, bool>> where, Expression<Func<T, float?>> avg)
        {
            return await _dbContext.Set<T>().Where<T>(where).AverageAsync<T>(avg);
        }
        public virtual async Task<decimal?> GetAvgAsync(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> avg)
        {
            return await _dbContext.Set<T>().Where<T>(where).AverageAsync<T>(avg);
        }

        #endregion

        #region 查最大

        public virtual async Task<int?> GetMaxAsync(Expression<Func<T, int?>> max)
        {
            return await _dbContext.Set<T>().MaxAsync<T, int?>(max);
        }
        public virtual async Task<double?> GetMaxAsync(Expression<Func<T, double?>> max)
        {
            return await _dbContext.Set<T>().MaxAsync<T, double?>(max);
        }
        public virtual async Task<decimal?> GetMaxAsync(Expression<Func<T, decimal?>> max)
        {
            return await _dbContext.Set<T>().MaxAsync<T, decimal?>(max);
        }
        public virtual async Task<DateTime?> GetMaxAsync(Expression<Func<T, DateTime?>> max)
        {
            return await _dbContext.Set<T>().MaxAsync<T, DateTime?>(max);
        }

        #endregion

        #region 查最小

        public virtual async Task<int?> GetMinAsync(Expression<Func<T, int?>> min)
        {
            return await _dbContext.Set<T>().MinAsync<T, int?>(min);
        }
        public virtual async Task<double?> GetMinAsync(Expression<Func<T, double?>> min)
        {
            return await _dbContext.Set<T>().MinAsync<T, double?>(min);
        }
        public virtual async Task<decimal?> GetMinAsync(Expression<Func<T, decimal?>> min)
        {
            return await _dbContext.Set<T>().MinAsync<T, decimal?>(min);
        }
        public virtual async Task<DateTime?> GetMinAsync(Expression<Func<T, DateTime?>> min)
        {
            return await _dbContext.Set<T>().MinAsync<T, DateTime?>(min);
        }

        #endregion

        #region SQL执行

        /// <summary>
        /// 异步执行sql语句
        /// </summary>
        public virtual async Task ExecuteAsync(string sql)
        {
            await _dbContext.Database.ExecuteSqlRawAsync(sql);
        }
        public virtual async Task ExecuteAsync(string sql, object parameters)
        {
            await _dbContext.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        /// <summary>
        /// 使用sql脚本异步查询实体列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual async Task<List<T>> QueryAsync(string sql)
        {
            return await _dbContext.Set<T>().FromSqlRaw<T>(sql).ToListAsync();
        }
        public virtual async Task<List<T>> QueryAsync(string sql, object parameters)
        {
            return await _dbContext.Set<T>().FromSqlRaw<T>(sql, parameters).ToListAsync();
        }

        #endregion

        #endregion
    }
}
