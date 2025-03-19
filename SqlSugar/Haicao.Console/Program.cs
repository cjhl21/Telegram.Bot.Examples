namespace Haicao.Console;

using SqlSugar;
using Console = System.Console;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        SqlSugarClient sqlSugarClient = GetClient();

        sqlSugarClient.DbFirst.CreateClassFile("../../../SqlSugarEntity");
    }

    public static SqlSugarClient GetClient()
    {
        ConnectionConfig connection = new ConnectionConfig()
        {
            ConnectionString = "Data Source=154.204.45.171,1433;Initial Catalog=SysLogDB;User ID=adminnet;Password=JYdb1234;Encrypt=True;TrustServerCertificate=True;",
            DbType = DbType.SqlServer, //指定数据库的类型
            IsAutoCloseConnection = true,
            InitKeyType = InitKeyType.Attribute
        };
        return new SqlSugarClient(connection, db =>
        {
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                //获取原生SQL推荐 5.1.4.63  性能OK
                Console.WriteLine(UtilMethods.GetNativeSql(sql, pars));
                //获取无参数化SQL 对性能有影响，特别大的SQL参数多的，调试使用
                //Console.WriteLine(UtilMethods.GetSqlString(DbType.SqlServer,sql,pars))
            };
        });
    }

}
