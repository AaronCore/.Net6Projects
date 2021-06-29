using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SysRepository
{
    public interface IRepository<T> where T : class, new()
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
        DbContextTransaction Transaction { get; set; }
        /// <summary>
        /// 事务提交结果
        /// </summary>
        bool Committed { get; set; }
        /// <summary>
        /// 提交事务
        /// </summary>
        void Commit();
        /// <summary>
        /// 回滚事务
        /// </summary>
        void Rollback();

        #endregion

        #region 添加

        bool Insert(T entity, bool isCommit = true);
        bool Insert(IEnumerable<T> entitys, bool isCommit = true);

        Task<bool> InsertAsync(T entity, bool isCommit = true);
        Task<bool> InsertAsync(IEnumerable<T> entitys, bool isCommit = true);

        #endregion

        #region 修改

        bool Update(T entity, bool isCommit = true);
        bool Update(IEnumerable<T> entitys, bool isCommit = true);

        Task<bool> UpdateAsync(T entity, bool isCommit = true);
        Task<bool> UpdateAsync(IEnumerable<T> entitys, bool isCommit = true);

        #endregion

        #region 删除

        bool Delete(T entity, bool isCommit = true);
        bool Delete(Expression<Func<T, bool>> filter, bool isCommit = true);

        Task<bool> DeleteAsync(T entity, bool isCommit = true);
        Task<bool> DeleteAsync(Expression<Func<T, bool>> filter, bool isCommit = true);

        #endregion

        #region 查询

        T Find(object id);
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAll(Expression<Func<T, bool>> where);
        T Get(Expression<Func<T, bool>> where);
        bool Any(Expression<Func<T, bool>> where);

        Task<T> FindAsync(object id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> where);
        Task<T> GetAsync(Expression<Func<T, bool>> where);
        Task<bool> AnyAsync(Expression<Func<T, bool>> where);

        #region 分页查询

        IEnumerable<T> PagedQuery(Expression<Func<T, bool>> where, Expression<Func<T, string>> orderby, string asc, int pageIndex, int pageSize);
        IEnumerable<T> PagedQuery(Expression<Func<T, bool>> where, Expression<Func<T, int?>> orderby, string asc, int pageIndex, int pageSize);
        IEnumerable<T> PagedQuery(Expression<Func<T, bool>> where, Expression<Func<T, DateTime?>> orderby, string asc, int pageIndex, int pageSize);
        IEnumerable<T> PagedQuery(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> orderby, string asc, int pageIndex, int pageSize);
        IEnumerable<T> PagedQuery(Expression<Func<T, bool>> where, Expression<Func<T, bool?>> orderby, string asc, int pageIndex, int pageSize);

        Task<IEnumerable<T>> PagedQueryAsync(Expression<Func<T, bool>> where, Expression<Func<T, string>> orderby, string asc, int pageIndex, int pageSize);
        Task<IEnumerable<T>> PagedQueryAsync(Expression<Func<T, bool>> where, Expression<Func<T, int?>> orderby, string asc, int pageIndex, int pageSize);
        Task<IEnumerable<T>> PagedQueryAsync(Expression<Func<T, bool>> where, Expression<Func<T, DateTime?>> orderby, string asc, int pageIndex, int pageSize);
        Task<IEnumerable<T>> PagedQueryAsync(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> orderby, string asc, int pageIndex, int pageSize);

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

        IEnumerable<T> SqlQuery(string sql, params object[] parameters);
        int ExecuteSql(string sql, params object[] parameters);
        Task<int> ExecuteSqlAsync(string sql, params object[] parameters);

        #endregion
    }
}
