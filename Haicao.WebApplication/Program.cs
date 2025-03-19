
using System.Data;
using Haicao.Services.IService;
using Haicao.Services.Service;
using Microsoft.AspNetCore;
using SqlSugar;
using Telegram.Bot;
using Builder = Microsoft.AspNetCore.Builder;

namespace Haicao.WebApplication;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Builder.WebApplication.CreateBuilder(args);

        builder.Logging.AddLog4Net("CfgFile/log4net.Config");
        builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
        // Setup bot configuration
        var botConfigSection = builder.Configuration.GetSection("BotConfiguration");
        builder.Services.Configure<BotConfiguration>(botConfigSection);
        builder.Services.AddHttpClient("tgwebhook").RemoveAllLoggers().AddTypedClient<ITelegramBotClient>(
            httpClient => new TelegramBotClient(botConfigSection.Get<BotConfiguration>()!.BotToken, httpClient));
        builder.Services.AddSingleton<Services.UpdateHandler>();
        builder.Services.ConfigureTelegramBotMvc();

        bool IsProduction = builder.Environment.EnvironmentName == "Production";// Development,Production
        string connStr = builder.Configuration.GetConnectionString(IsProduction ? "Default" : "Test")!;
        builder.Services.AddTransient<ISqlSugarClient>(serverprivider =>
        {
            ConnectionConfig connection = new ConnectionConfig()
            {
                ConnectionString = connStr,
                DbType = SqlSugar.DbType.SqlServer,
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
        });

        builder.Services.AddTransient<ITgUpdateService, TgUpdateService>();

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }


        // Configure the HTTP request pipeline.

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
