using Haicao.Entity;
using SqlSugar;
using System.Linq.Expressions;

namespace Haicao.Services.IService
{
    public interface IBaseService
    {
        #region 自定义
        /// <summary>
        /// 根据条件可批量更新某个字段
        /// </summary>
        /// <typeparam name="T">实体类名称</typeparam>
        /// <param name="columns">更新字段</param>
        /// <param name="expression">查询条件</param>
        /// <returns></returns>
        int UpdateColumns<T>(Expression<Func<T, bool>> columns, Expression<Func<T, bool>> expression) where T : class, new();

        int InsertNew<T>(T t) where T : class, new();
        /// <summary>
        /// 批量新增数据
        /// </summary>
        /// <typeparam name="T">实体类名称</typeparam>
        /// <param name="list">实体列表</param>
        /// <returns></returns>
        int BatchInsert<T>(List<T> list) where T : class, new();

        /// <summary>
        /// 根据条件可批量删除数据
        /// </summary>
        /// <typeparam name="T">实体类名称</typeparam>
        /// <param name="expression">批量删除的条件</param>
        /// <returns></returns>
        int BatchDelete<T>(Expression<Func<T, bool>> expression) where T : class, new();
        #endregion

        #region Query
        /// <summary>
        /// 主键查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T Find<T>(int id) where T : class;

        /// <summary>
        /// 条件查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="funcWhere"></param>
        /// <returns></returns>
        ISugarQueryable<T> Query<T>(Expression<Func<T, bool>> funcWhere) where T : class;

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="funcWhere"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="funcOrderby"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        PagingData<T> QueryPage<T>(Expression<Func<T, bool>> funcWhere, int pageSize, int pageIndex, Expression<Func<T, object>> funcOrderby, bool isAsc = true) where T : class;
        #endregion

        #region Add

        /// <summary>
        /// 新增数据-同步版本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        T Insert<T>(T t) where T : class, new();

        /// <summary>
        /// 新增数据-异步版本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        Task<T> InsertAsync<T>(T t) where T : class, new();

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tList"></param>
        /// <returns></returns>
        Task<bool> InsertList<T>(List<T> tList) where T : class, new();
        #endregion

        #region Update
        int UpdateByFieldName<T>(T t, string fieldName) where T : class, new();
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync<T>(T t) where T : class, new();
        int Update<T>(T t) where T : class, new();
        /// <summary>
        /// 更新数据，即时Commit
        /// </summary>
        /// <param name="tList"></param>
        void Update<T>(List<T> tList) where T : class, new();
        #endregion

        #region Delete
        /// <summary>
        /// 根据主键删除数据
        /// </summary>
        bool Delete<T>(object pId) where T : class, new();

        /// <su+mary>
        /// 删除数据，即时Commit
        /// </summary>
        /// <param name="t"></param>
        void Delete<T>(T t) where T : class, new();

        /// <summary>
        /// 删除数据，即时Commit
        /// </summary>
        /// <param name="tList"></param>
        void Delete<T>(List<T> tList) where T : class;
        #endregion

        #region Other

        /// <summary>
        /// 执行sql 返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        ISugarQueryable<T> ExcuteQuery<T>(string sql) where T : class, new();

        #endregion
    }
}
