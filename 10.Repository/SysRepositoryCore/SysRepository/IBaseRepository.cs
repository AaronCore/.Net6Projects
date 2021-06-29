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
        DbSet<T> DbSet { get; }
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

        void Insert(T entity);
        void Insert(IEnumerable<T> entity);

        Task InsertAsync(T entity);
        Task InsertAsync(IEnumerable<T> entity);

        #endregion

        #region 修改

        void Update(T entity);
        int Update(Expression<Func<T, bool>> where, Expression<Func<T, T>> entity);
        Task<int> UpdateAsync(Expression<Func<T, bool>> where, Expression<Func<T, T>> entity);

        #endregion

        #region 删除

        int Delete(Expression<Func<T, bool>> where);
        Task<int> DeleteAsync(Expression<Func<T, bool>> where);

        #endregion

        #region 查询

        T Find(object id);
        T Get(Expression<Func<T, bool>> where);
        List<T> GetAll();
        List<T> GetAll(Expression<Func<T, bool>> where);
        bool Any(Expression<Func<T, bool>> where);

        Task<T> FindAsync(object id);
        Task<T> GetAsync(Expression<Func<T, bool>> where);
        Task<List<T>> GetAllAsync();
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> where);
        Task<bool> AnyAsync(Expression<Func<T, bool>> where);

        #region 分页

        IEnumerable<T> PagedQuery(Expression<Func<T, bool>> where, Expression<Func<T, string>> orderby, string asc, int pageIndex, int pageSize);
        IEnumerable<T> PagedQuery(Expression<Func<T, bool>> where, Expression<Func<T, int?>> orderby, string asc, int pageIndex, int pageSize);
        IEnumerable<T> PagedQuery(Expression<Func<T, bool>> where, Expression<Func<T, DateTime?>> orderby, string asc, int pageIndex, int pageSize);
        IEnumerable<T> PagedQuery(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> orderby, string asc, int pageIndex, int pageSize);
        IEnumerable<T> PagedQuery(Expression<Func<T, bool>> where, Expression<Func<T, bool?>> orderby, string asc, int pageIndex, int pageSize);

        Task<List<T>> PagedQueryAsync(Expression<Func<T, bool>> where, Expression<Func<T, string>> orderby, string asc, int pageIndex, int pageSize);
        Task<List<T>> PagedQueryAsync(Expression<Func<T, bool>> where, Expression<Func<T, int?>> orderby, string asc, int pageIndex, int pageSize);
        Task<List<T>> PagedQueryAsync(Expression<Func<T, bool>> where, Expression<Func<T, DateTime?>> orderby, string asc, int pageIndex, int pageSize);
        Task<List<T>> PagedQueryAsync(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> orderby, string asc, int pageIndex, int pageSize);

        #endregion

        #region 查询实体数量

        int Count(Expression<Func<T, bool>> where);
        Task<int> CountAsync(Expression<Func<T, bool>> where);

        #endregion

        #region 求平均，求总计

        int? Sum(Expression<Func<T, bool>> where, Expression<Func<T, int?>> sum);
        double? Sum(Expression<Func<T, bool>> where, Expression<Func<T, double?>> sum);
        float? Sum(Expression<Func<T, bool>> where, Expression<Func<T, float?>> sum);
        decimal? Sum(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> sum);
        double? Avg(Expression<Func<T, bool>> where, Expression<Func<T, int?>> avg);
        double? Avg(Expression<Func<T, bool>> where, Expression<Func<T, double?>> avg);
        float? Avg(Expression<Func<T, bool>> where, Expression<Func<T, float?>> avg);
        decimal? Avg(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> avg);

        Task<int?> SumAsync(Expression<Func<T, bool>> where, Expression<Func<T, int?>> sum);
        Task<double?> SumAsync(Expression<Func<T, bool>> where, Expression<Func<T, double?>> sum);
        Task<float?> SumAsync(Expression<Func<T, bool>> where, Expression<Func<T, float?>> sum);
        Task<decimal?> SumAsync(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> sum);
        Task<double?> AvgAsync(Expression<Func<T, bool>> where, Expression<Func<T, int?>> avg);
        Task<double?> AvgAsync(Expression<Func<T, bool>> where, Expression<Func<T, double?>> avg);
        Task<float?> AvgAsync(Expression<Func<T, bool>> where, Expression<Func<T, float?>> avg);
        Task<decimal?> AvgAsync(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> avg);

        #endregion

        #region 查最大

        int? Max(Expression<Func<T, int?>> max);
        double? Max(Expression<Func<T, double?>> max);
        decimal? Max(Expression<Func<T, decimal?>> max);
        DateTime? Max(Expression<Func<T, DateTime?>> max);

        Task<int?> MaxAsync(Expression<Func<T, int?>> max);
        Task<double?> MaxAsync(Expression<Func<T, double?>> max);
        Task<decimal?> MaxAsync(Expression<Func<T, decimal?>> max);
        Task<DateTime?> MaxAsync(Expression<Func<T, DateTime?>> max);

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
