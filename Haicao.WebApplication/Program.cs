
using Microsoft.AspNetCore;
using Telegram.Bot;
using Builder = Microsoft.AspNetCore.Builder;

namespace Haicao.WebApplication;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Builder.WebApplication.CreateBuilder(args);

        // Setup bot configuration
        var botConfigSection = builder.Configuration.GetSection("BotConfiguration");
        builder.Services.Configure<BotConfiguration>(botConfigSection);
        builder.Services.AddHttpClient("tgwebhook").RemoveAllLoggers().AddTypedClient<ITelegramBotClient>(
            httpClient => new TelegramBotClient(botConfigSection.Get<BotConfiguration>()!.BotToken, httpClient));
        builder.Services.AddSingleton<Services.UpdateHandler>();
        builder.Services.ConfigureTelegramBotMvc();

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
