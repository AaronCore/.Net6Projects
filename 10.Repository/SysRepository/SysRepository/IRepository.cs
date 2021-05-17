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
        DbSet<T> dbSet { get; }
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

        bool AddEntity(T entity, bool isCommit = true);
        bool AddEntityRange(IEnumerable<T> entitys, bool isCommit = true);

        Task<bool> AddEntityAsync(T entity, bool isCommit = true);
        Task<bool> AddEntityRangeAsync(IEnumerable<T> entitys, bool isCommit = true);

        #endregion

        #region 修改

        bool UpdateEntity(T entity, bool isCommit = true);
        bool UpdateEntityRange(IEnumerable<T> entitys, bool isCommit = true);

        Task<bool> UpdateAsync(T entity, bool isCommit = true);
        Task<bool> UpdateEntityRangeAsync(IEnumerable<T> entitys, bool isCommit = true);

        #endregion

        #region 删除

        bool DelEntity(T entity, bool isCommit = true);
        bool DelEntityRange(Expression<Func<T, bool>> filter, bool isCommit = true);

        Task<bool> DelEntityAsync(T entity, bool isCommit = true);
        Task<bool> DelEntityRangeAsync(Expression<Func<T, bool>> filter, bool isCommit = true);

        #endregion

        #region 查询

        T Find(object id);
        IEnumerable<T> GetEntityAll();
        IEnumerable<T> GetEntityAll(Expression<Func<T, bool>> where);
        T GetEntity(Expression<Func<T, bool>> where);
        bool AnyEntity(Expression<Func<T, bool>> where);

        Task<T> FindAsync(object id);
        Task<IEnumerable<T>> GetEntityAllAsync();
        Task<IEnumerable<T>> GetEntityAllAsync(Expression<Func<T, bool>> where);
        Task<T> GetEntityAsync(Expression<Func<T, bool>> where);
        Task<bool> AnyEntityAsync(Expression<Func<T, bool>> where);

        #region 分页查询

        IEnumerable<T> LoadEntityEnumerable(Expression<Func<T, bool>> where, Expression<Func<T, string>> orderby, string asc, int pageIndex, int pageSize);
        IEnumerable<T> LoadEntityEnumerable(Expression<Func<T, bool>> where, Expression<Func<T, int?>> orderby, string asc, int pageIndex, int pageSize);
        IEnumerable<T> LoadEntityEnumerable(Expression<Func<T, bool>> where, Expression<Func<T, DateTime?>> orderby, string asc, int pageIndex, int pageSize);
        IEnumerable<T> LoadEntityEnumerable(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> orderby, string asc, int pageIndex, int pageSize);
        IEnumerable<T> LoadEntityEnumerable(Expression<Func<T, bool>> where, Expression<Func<T, bool?>> orderby, string asc, int pageIndex, int pageSize);

        Task<IEnumerable<T>> LoadEntityListAsync(Expression<Func<T, bool>> where, Expression<Func<T, string>> orderby, string asc, int pageIndex, int pageSize);
        Task<IEnumerable<T>> LoadEntityListAsync(Expression<Func<T, bool>> where, Expression<Func<T, int?>> orderby, string asc, int pageIndex, int pageSize);
        Task<IEnumerable<T>> LoadEntityListAsync(Expression<Func<T, bool>> where, Expression<Func<T, DateTime?>> orderby, string asc, int pageIndex, int pageSize);
        Task<IEnumerable<T>> LoadEntityListAsync(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> orderby, string asc, int pageIndex, int pageSize);

        #endregion

        #region 查询实体数量

        int GetEntityCount(Expression<Func<T, bool>> where);

        Task<int> GetEntityCountAsync(Expression<Func<T, bool>> where);

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

        IEnumerable<T> SqlQuery(string sql, params object[] parameters);
        int ExecuteSqlCommand(string sql, params object[] parameters);
        Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters);

        #endregion
    }
}
