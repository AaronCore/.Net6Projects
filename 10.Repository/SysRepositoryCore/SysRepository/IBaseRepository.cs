using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SysEntityFrameworkCore;

namespace SysRepository
{
    /// <summary>
    /// 仓储基类中定义的公共的方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBaseRepository<T> : IUnitOfWork where T : class, new()
    {
        #region 数据对象操作

        /// <summary>
        /// 数据上下文
        /// </summary>
        DbContext Context { get; }
        /// <summary>
        /// 数据模型操作
        /// </summary>
        DbSet<T> dbSet { get; }
        /// <summary>
        /// EF事务
        /// </summary>
        DbTransaction Transaction { get; set; }
        /// <summary>
        /// 事务提交结果
        /// </summary>
        bool Committed { get; set; }
        /// <summary>
        /// 提交事务
        /// </summary>
        void CommitAffair();
        /// <summary>
        /// 回滚事务
        /// </summary>
        void RollbackAffair();

        #endregion

        #region 新增

        void AddEntity(T entity);
        void AddRange(IEnumerable<T> entity);

        Task AddEntityAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entity);

        #endregion

        #region 修改

        void UpdateEntity(T entity);
        int UpdateEntityRange(Expression<Func<T, bool>> where, Expression<Func<T, T>> entity);
        Task<int> UpdateEntityRangeAsync(Expression<Func<T, bool>> where, Expression<Func<T, T>> entity);

        #endregion

        #region 删除

        int DelEntity(Expression<Func<T, bool>> where);
        Task<int> DelEntityAsync(Expression<Func<T, bool>> where);

        #endregion

        #region 查询

        T Find(object id);
        T GetEntity(Expression<Func<T, bool>> where);
        List<T> GetEntityAll();
        List<T> GetEntityAll(Expression<Func<T, bool>> where);
        bool AnyEntity(Expression<Func<T, bool>> where);

        Task<T> FindAsync(object id);
        Task<T> GetEntityAsync(Expression<Func<T, bool>> where);
        Task<List<T>> GetEntityAllAsync();
        Task<List<T>> GetEntityAllAsync(Expression<Func<T, bool>> where);
        Task<bool> AnyEntityAsync(Expression<Func<T, bool>> where);

        #region 分页

        IEnumerable<T> LoadEntityEnumerable(Expression<Func<T, bool>> where, Expression<Func<T, string>> orderby, string asc, int pageIndex, int pageSize);
        IEnumerable<T> LoadEntityEnumerable(Expression<Func<T, bool>> where, Expression<Func<T, int?>> orderby, string asc, int pageIndex, int pageSize);
        IEnumerable<T> LoadEntityEnumerable(Expression<Func<T, bool>> where, Expression<Func<T, DateTime?>> orderby, string asc, int pageIndex, int pageSize);
        IEnumerable<T> LoadEntityEnumerable(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> orderby, string asc, int pageIndex, int pageSize);
        IEnumerable<T> LoadEntityEnumerable(Expression<Func<T, bool>> where, Expression<Func<T, bool?>> orderby, string asc, int pageIndex, int pageSize);

        Task<List<T>> LoadEntityListAsync(Expression<Func<T, bool>> where, Expression<Func<T, string>> orderby, string asc, int pageIndex, int pageSize);
        Task<List<T>> LoadEntityListAsync(Expression<Func<T, bool>> where, Expression<Func<T, int?>> orderby, string asc, int pageIndex, int pageSize);
        Task<List<T>> LoadEntityListAsync(Expression<Func<T, bool>> where, Expression<Func<T, DateTime?>> orderby, string asc, int pageIndex, int pageSize);
        Task<List<T>> LoadEntityListAsync(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> orderby, string asc, int pageIndex, int pageSize);

        #endregion

        #region 查询实体数量

        int GetEntitysCount(Expression<Func<T, bool>> where);
        Task<int> GetEntitysCountAsync(Expression<Func<T, bool>> where);

        #endregion

        #region 求平均，求总计

        int? GetSum(Expression<Func<T, bool>> where, Expression<Func<T, int?>> sum);
        double? GetSum(Expression<Func<T, bool>> where, Expression<Func<T, double?>> sum);
        float? GetSum(Expression<Func<T, bool>> where, Expression<Func<T, float?>> sum);
        decimal? GetSum(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> sum);
        double? GetAvg(Expression<Func<T, bool>> where, Expression<Func<T, int?>> avg);
        double? GetAvg(Expression<Func<T, bool>> where, Expression<Func<T, double?>> avg);
        float? GetAvg(Expression<Func<T, bool>> where, Expression<Func<T, float?>> avg);
        decimal? GetAvg(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> avg);

        Task<int?> GetSumAsync(Expression<Func<T, bool>> where, Expression<Func<T, int?>> sum);
        Task<double?> GetSumAsync(Expression<Func<T, bool>> where, Expression<Func<T, double?>> sum);
        Task<float?> GetSumAsync(Expression<Func<T, bool>> where, Expression<Func<T, float?>> sum);
        Task<decimal?> GetSumAsync(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> sum);
        Task<double?> GetAvgAsync(Expression<Func<T, bool>> where, Expression<Func<T, int?>> avg);
        Task<double?> GetAvgAsync(Expression<Func<T, bool>> where, Expression<Func<T, double?>> avg);
        Task<float?> GetAvgAsync(Expression<Func<T, bool>> where, Expression<Func<T, float?>> avg);
        Task<decimal?> GetAvgAsync(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> avg);

        #endregion

        #region 查最大

        int? GetMax(Expression<Func<T, int?>> max);
        double? GetMax(Expression<Func<T, double?>> max);
        decimal? GetMax(Expression<Func<T, decimal?>> max);
        DateTime? GetMax(Expression<Func<T, DateTime?>> max);

        Task<int?> GetMaxAsync(Expression<Func<T, int?>> max);
        Task<double?> GetMaxAsync(Expression<Func<T, double?>> max);
        Task<decimal?> GetMaxAsync(Expression<Func<T, decimal?>> max);
        Task<DateTime?> GetMaxAsync(Expression<Func<T, DateTime?>> max);

        #endregion

        #endregion

        #region SQL执行

        void ExecuteSql(string sql, params object[] parameters);
        List<T> QuerySql(string sql, params object[] parameters);

        Task ExecuteSqlAsync(string sql, params object[] parameters);
        Task<List<T>> QuerySqlAsync(string sql, params object[] parameters);

        #endregion
    }
}
