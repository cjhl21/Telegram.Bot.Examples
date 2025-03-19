using Haicao.Entity;
using Haicao.Services.IService;
using SqlSugar;
using System.Linq.Expressions;
using System.Security.Cryptography;

namespace Haicao.Services.Service
{
    public abstract class BaseService : IBaseService
    {
        /// <summary>
        /// 
        /// </summary>
        protected ISqlSugarClient _Client { get; set; }

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="context"></param>
        public BaseService(ISqlSugarClient client)
        {
            _Client = client;
        }

        #region 自定义
        /// <summary>
        /// 根据条件可批量更新某个字段
        /// </summary>
        /// <typeparam name="T">实体类名称</typeparam>
        /// <param name="columns">更新字段</param>
        /// <param name="expression">查询条件</param>
        /// <returns></returns>
        public int UpdateColumns<T>(Expression<Func<T, bool>> columns, Expression<Func<T, bool>> expression) where T : class, new()
        {
            return _Client.Updateable<T>().SetColumns(columns)
                                     .Where(expression)
                                     .ExecuteCommand();
        }

        public int InsertNew<T>(T t) where T : class, new()
        {
            return _Client.Insertable(t).ExecuteReturnIdentity();
        }
        /// <summary>
        /// 批量新增数据
        /// </summary>
        /// <typeparam name="T">实体类名称</typeparam>
        /// <param name="list">实体列表</param>
        /// <returns></returns>
        public int BatchInsert<T>(List<T> list) where T : class, new()
        {
            return _Client.Insertable(list).PageSize(100).ExecuteCommand();
        }

        /// <summary>
        /// 根据条件可批量删除数据
        /// </summary>
        /// <typeparam name="T">实体类名称</typeparam>
        /// <param name="expression">批量删除的条件</param>
        /// <returns></returns>
        public int BatchDelete<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            return _Client.Deleteable<T>().Where(expression).ExecuteCommand();
        }
        #endregion


        #region Query

        /// <summary>
        /// 主键查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public T Find<T>(int id) where T : class
        {
            return _Client.Queryable<T>().InSingle(id);
        }

        /// <summary>
        /// 条件查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="funcWhere"></param>
        /// <returns></returns>
        public ISugarQueryable<T> Query<T>(Expression<Func<T, bool>> funcWhere) where T : class
        {
            return _Client.Queryable<T>().Where(funcWhere);
        }

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
        public PagingData<T> QueryPage<T>(Expression<Func<T, bool>> funcWhere, int pageSize, int pageIndex, Expression<Func<T, object>> funcOrderby, bool isAsc = true) where T : class
        {
            var list = _Client.Queryable<T>();
            if (funcWhere != null)
                list = list.Where(funcWhere);
            list = list.OrderByIF(true, funcOrderby, isAsc ? OrderByType.Asc : OrderByType.Desc);
            var result = new PagingData<T>()
            {
                DataList = list.ToPageList(pageIndex, pageSize),
                PageIndex = pageIndex,
                PageSize = pageSize,
                RecordCount = list.Count(),
            };
            return result;
        }
        #endregion

        #region Insert

       
        /// <summary>
        /// 新增数据-同步版本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public T Insert<T>(T t) where T : class, new()
        {
            return _Client.Insertable(t).ExecuteReturnEntity();
        }

        /// <summary>
        /// 新增数据-异步版本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<T> InsertAsync<T>(T t) where T : class, new()
        {
            return await _Client.Insertable(t).ExecuteReturnEntityAsync();
        }

        /// <summary>
        /// 批量新增-事务执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tList"></param>
        /// <returns></returns>
        public async Task<bool> InsertList<T>(List<T> tList) where T : class, new()
        {
            return await _Client.Insertable(tList).ExecuteCommandIdentityIntoEntityAsync();
        }
        #endregion

        #region Update
        public int UpdateByFieldName<T>(T t, string fieldName) where T : class, new()
        {
            if (t == null) throw new Exception("t is null");

            return _Client.Updateable(t).WhereColumns(fieldName).ExecuteCommand();
        }
        /// <summary>
        /// 是没有实现查询，直接更新的,需要Attach和State
        /// 
        /// 如果是已经在context，只能再封装一个(在具体的service)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public async Task<bool> UpdateAsync<T>(T t) where T : class, new()
        {
            if (t == null) throw new Exception("t is null");

            return await _Client.Updateable(t).WhereColumns().ExecuteCommandHasChangeAsync();
        }
        public int Update<T>(T t) where T : class, new()
        {
            return _Client.Updateable(t).ExecuteCommand();
        }
        public void Update<T>(List<T> tList) where T : class, new()
        {
            _Client.Updateable(tList).ExecuteCommand();
        }

        #endregion

        #region Delete
        /// <summary>
        /// 根据主键删除
        /// </summary>
        public bool Delete<T>(object pId) where T : class, new()
        {
            return _Client.Deleteable<T>().In(pId).ExecuteCommandHasChange();
        }
        public void Delete<T>(T t) where T : class, new()
        {
            _Client.Deleteable(t).ExecuteCommand();
        }
        public void Delete<T>(List<T> tList) where T : class
        {
            _Client.Deleteable(tList).ExecuteCommand();
        }
        #endregion


        #region Other 
        ISugarQueryable<T> IBaseService.ExcuteQuery<T>(string sql) where T : class
        {
            return _Client.SqlQueryable<T>(sql);
        }
        public void Dispose()
        {
            if (_Client != null)
                _Client.Dispose();
        }
        #endregion

    }
}
