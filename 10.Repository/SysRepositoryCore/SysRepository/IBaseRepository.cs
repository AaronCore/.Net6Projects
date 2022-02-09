using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SysEntityFrameworkCore;

namespace SysRepository
{
    /// <summary>
    /// 仓储基类中定义的公共的方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBaseRepository<T> : IUnitOfWork where T : class, new()
    {
        #region 同步

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        void Add(T entity);

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        void AddRange(List<T> entity);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        int Delete(Expression<Func<T, bool>> where);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        void Update(T entity);

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="where"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        int UpdateRange(Expression<Func<T, bool>> where, Expression<Func<T, T>> entity);

        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="where">lamda表达式</param>
        /// <returns></returns>
        IEnumerable<T> Get(Expression<Func<T, bool>> where);

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="where">lamda表达式</param>
        /// <returns></returns>
        T? GetEntity(Expression<Func<T, bool>> where);

        /// <summary>
        /// 根据主键获取
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        T? Find(object? key);

        /// <summary>
        /// 获取最大的一条数据
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        T? GetMaxSort(Expression<Func<T, int>> order);

        /// <summary>
        /// 获取最小的一条数据
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        T? GetMinSort(Expression<Func<T, int>> order);

        /// <summary>
        ///  Any 查找数据判断数据是否存在
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        bool Any(Expression<Func<T, bool>> where);

        /// <summary>
        /// 查询数量
        /// </summary>
        /// <returns></returns>
        int GetCount();

        /// <summary>
        /// 查询数量
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        int GetCount(Expression<Func<T, bool>> where);

        #region 分页

        List<T> Load(Expression<Func<T, bool>> where, Expression<Func<T, string>> orderby, string asc, int pageIndex, int pageSize);
        List<T> Load(Expression<Func<T, bool>> where, Expression<Func<T, int?>> orderby, string asc, int pageIndex, int pageSize);
        List<T> Load(Expression<Func<T, bool>> where, Expression<Func<T, DateTime?>> orderby, string asc, int pageIndex, int pageSize);
        List<T> Load(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> orderby, string asc, int pageIndex, int pageSize);
        List<T> Load(Expression<Func<T, bool>> where, Expression<Func<T, bool?>> orderby, string asc, int pageIndex, int pageSize);

        #endregion

        #region 求总计

        int? GetSum(Expression<Func<T, bool>> where, Expression<Func<T, int?>> sum);
        double? GetSum(Expression<Func<T, bool>> where, Expression<Func<T, double?>> sum);
        float? GetSum(Expression<Func<T, bool>> where, Expression<Func<T, float?>> sum);
        decimal? GetSum(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> sum);

        #endregion

        #region 求平均

        double? GetAvg(Expression<Func<T, bool>> where, Expression<Func<T, int?>> avg);
        double? GetAvg(Expression<Func<T, bool>> where, Expression<Func<T, double?>> avg);
        float? GetAvg(Expression<Func<T, bool>> where, Expression<Func<T, float?>> avg);
        decimal? GetAvg(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> avg);

        #endregion

        #region 查最大

        int? GetMax(Expression<Func<T, int?>> max);
        double? GetMax(Expression<Func<T, double?>> max);
        decimal? GetMax(Expression<Func<T, decimal?>> max);
        DateTime? GetMax(Expression<Func<T, DateTime?>> max);

        #endregion

        #region 查最小

        int? GetMin(Expression<Func<T, int?>> min);
        double? GetMin(Expression<Func<T, double?>> min);
        decimal? GetMin(Expression<Func<T, decimal?>> min);
        DateTime? GetMin(Expression<Func<T, DateTime?>> min);

        #endregion

        #region SQL执行

        /// <summary>
        /// 执行sql
        /// </summary>
        /// <param name="sql"></param>
        void Execute(string sql);
        void Execute(string sql, object parameters);

        /// <summary>
        /// 使用sql脚本查询实体列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        List<T> Query(string sql);
        List<T> Query(string sql, object parameters);

        #endregion

        #endregion

        #region 异步

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task AddAsync(T entity);

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task AddRangeAsync(List<T> entity);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="where">实体</param>
        /// <returns></returns>
        Task<int> DeleteAsync(Expression<Func<T, bool>> where);

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="where"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> UpdateRangeAsync(Expression<Func<T, bool>> where, Expression<Func<T, T>> entity);

        /// <summary>
        /// 异步查询所有数据
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> where);

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="where">lamda表达式</param>
        /// <returns></returns>
        Task<T?> GetEntityAsync(Expression<Func<T, bool>> where);

        /// <summary>
        /// 根据主键获取
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T?> FindAsync(object? key);

        /// <summary>
        /// 获取最大的一条数据
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task<T?> GetMaxSortAsync(Expression<Func<T, int>> order);

        /// <summary>
        /// 获取最小的一条数据
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task<T?> GetMinSortAsync(Expression<Func<T, int>> order);

        /// <summary>
        /// Any 查找数据判断数据是否存在
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        Task<bool> AnyAsync(Expression<Func<T, bool>> where);

        /// <summary>
        /// 查询数量
        /// </summary>
        /// <returns></returns>
        Task<int> GetCountAsync();

        /// <summary>
        /// 查询数量
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        Task<int> GetCountAsync(Expression<Func<T, bool>> where);

        #region 分页

        Task<List<T>> LoadAsync(Expression<Func<T, bool>> where, Expression<Func<T, string>> orderby, string asc, int pageIndex, int pageSize);
        Task<List<T>> LoadAsync(Expression<Func<T, bool>> where, Expression<Func<T, int?>> orderby, string asc, int pageIndex, int pageSize);
        Task<List<T>> LoadAsync(Expression<Func<T, bool>> where, Expression<Func<T, DateTime?>> orderby, string asc, int pageIndex, int pageSize);
        Task<List<T>> LoadAsync(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> orderby, string asc, int pageIndex, int pageSize);

        #endregion

        #region 求总计

        Task<int?> GetSumAsync(Expression<Func<T, bool>> where, Expression<Func<T, int?>> sum);
        Task<double?> GetSumAsync(Expression<Func<T, bool>> where, Expression<Func<T, double?>> sum);
        Task<float?> GetSumAsync(Expression<Func<T, bool>> where, Expression<Func<T, float?>> sum);
        Task<decimal?> GetSumAsync(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> sum);

        #endregion

        #region 求平均

        Task<double?> GetAvgAsync(Expression<Func<T, bool>> where, Expression<Func<T, int?>> avg);
        Task<double?> GetAvgAsync(Expression<Func<T, bool>> where, Expression<Func<T, double?>> avg);
        Task<float?> GetAvgAsync(Expression<Func<T, bool>> where, Expression<Func<T, float?>> avg);
        Task<decimal?> GetAvgAsync(Expression<Func<T, bool>> where, Expression<Func<T, decimal?>> avg);

        #endregion

        #region 查最大

        Task<int?> GetMaxAsync(Expression<Func<T, int?>> max);
        Task<double?> GetMaxAsync(Expression<Func<T, double?>> max);
        Task<decimal?> GetMaxAsync(Expression<Func<T, decimal?>> max);
        Task<DateTime?> GetMaxAsync(Expression<Func<T, DateTime?>> max);

        #endregion

        #region 查最小

        Task<int?> GetMinAsync(Expression<Func<T, int?>> min);
        Task<double?> GetMinAsync(Expression<Func<T, double?>> min);
        Task<decimal?> GetMinAsync(Expression<Func<T, decimal?>> min);
        Task<DateTime?> GetMinAsync(Expression<Func<T, DateTime?>> min);

        #endregion

        #region SQL执行

        /// <summary>
        /// 异步执行sql
        /// </summary>
        /// <param name="sql"></param>
        Task ExecuteAsync(string sql);
        Task ExecuteAsync(string sql, object parameters);

        /// <summary>
        /// 使用sql脚本异步查询实体列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        Task<List<T>> QueryAsync(string sql);
        Task<List<T>> QueryAsync(string sql, object parameters);

        #endregion

        #endregion
    }
}
