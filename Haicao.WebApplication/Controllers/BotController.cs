using Haicao.Services.IService;
using Haicao.Services.Service;
using Haicao.WebApplication.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Newtonsoft.Json;
using Haicao.Entity.SqlSugarEntity;

namespace Haicao.WebApplication.Controllers;

[ApiController]
[Route("[controller]")]
public class BotController(ILogger<BotController> logger, IOptions<BotConfiguration> Config,ITgUpdateService tgUpdateService) : ControllerBase
{
    [HttpGet("setWebhook")]
    public async Task<string> SetWebHook([FromServices] ITelegramBotClient bot, CancellationToken ct)
    {
        logger.LogInformation("invoke SetWebHook");
        var webhookUrl = Config.Value.BotWebhookUrl.AbsoluteUri;
        await bot.SetWebhook(webhookUrl, allowedUpdates: [], secretToken: Config.Value.SecretToken, cancellationToken: ct);
        return $"Webhook set to {webhookUrl}";
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update update, [FromServices] ITelegramBotClient bot, [FromServices] UpdateHandler handleUpdateService, CancellationToken ct)
    {
        string JsonStr = JsonConvert.SerializeObject(update);
        logger.LogWarning("{Update}", JsonStr);
        if (Request.Headers["X-Telegram-Bot-Api-Secret-Token"] != Config.Value.SecretToken)
            return Forbid();
        try
        {
            #region tgUpdate
            tgUpdate model = new tgUpdate();
            model.Id = update.Id;
            model.FCreateTime = DateTime.Now;
            model.Type = (int)update.Type;
            model.FJsonStr = JsonStr;
            model.Message = update.Message is null ? "": JsonConvert.SerializeObject(update.Message);
            model.EditedMessage = update.EditedMessage is null ? "" : JsonConvert.SerializeObject(update.EditedMessage);
            model.ChannelPost = update.ChannelPost is null ? "" : JsonConvert.SerializeObject(update.ChannelPost);
            model.EditedChannelPost = update.EditedChannelPost is null ? "" : JsonConvert.SerializeObject(update.EditedChannelPost);
            model.BusinessConnection = update.BusinessConnection is null ? "" : JsonConvert.SerializeObject(update.BusinessConnection);
            model.BusinessMessage = update.BusinessMessage is null ? "" : JsonConvert.SerializeObject(update.BusinessMessage);
            model.EditedBusinessMessage = update.EditedBusinessMessage is null ? "" : JsonConvert.SerializeObject(update.EditedBusinessMessage);
            model.DeletedBusinessMessages = update.DeletedBusinessMessages is null ? "" : JsonConvert.SerializeObject(update.DeletedBusinessMessages);
            model.MessageReaction = update.MessageReaction is null ? "" : JsonConvert.SerializeObject(update.MessageReaction);
            model.MessageReactionCount = update.MessageReactionCount is null ? "" : JsonConvert.SerializeObject(update.MessageReactionCount);
            model.InlineQuery = update.InlineQuery is null ? "" : JsonConvert.SerializeObject(update.InlineQuery);
            model.ChosenInlineResult = update.ChosenInlineResult is null ? "" : JsonConvert.SerializeObject(update.ChosenInlineResult);
            model.CallbackQuery = update.CallbackQuery is null ? "" : JsonConvert.SerializeObject(update.CallbackQuery);
            model.ShippingQuery = update.ShippingQuery is null ? "" : JsonConvert.SerializeObject(update.ShippingQuery);
            model.PreCheckoutQuery = update.PreCheckoutQuery is null ? "" : JsonConvert.SerializeObject(update.PreCheckoutQuery);
            model.PurchasedPaidMedia = update.PurchasedPaidMedia is null ? "" : JsonConvert.SerializeObject(update.PurchasedPaidMedia);
            model.Poll = update.Poll is null ? "" : JsonConvert.SerializeObject(update.Poll);
            model.PollAnswer = update.PollAnswer is null ? "" : JsonConvert.SerializeObject(update.PollAnswer);
            model.MyChatMember = update.MyChatMember is null ? "" : JsonConvert.SerializeObject(update.MyChatMember);
            model.ChatMember = update.ChatMember is null ? "" : JsonConvert.SerializeObject(update.ChatMember);
            model.ChatJoinRequest = update.ChatJoinRequest is null ? "" : JsonConvert.SerializeObject(update.ChatJoinRequest);
            model.ChatBoost = update.ChatBoost is null ? "" : JsonConvert.SerializeObject(update.ChatBoost);
            model.RemovedChatBoost = update.RemovedChatBoost is null ? "" : JsonConvert.SerializeObject(update.RemovedChatBoost);
            tgUpdateService.Insert(model);
            #endregion



            await handleUpdateService.HandleUpdateAsync(bot, update, ct);
        }
        catch (Exception exception)
        {
            await handleUpdateService.HandleErrorAsync(bot, exception, Telegram.Bot.Polling.HandleErrorSource.HandleUpdateError, ct);
        }
        return Ok();
    }
}
