using Haicao.Services.IService;
using Haicao.Services.Service;
using Haicao.WebApplication.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Newtonsoft.Json;
using Haicao.Entity.SqlSugarEntity;
using static System.Net.Mime.MediaTypeNames;
using System;
using Telegram.Bot.Types.Passport;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;

namespace Haicao.WebApplication.Controllers;

[ApiController]
[Route("[controller]")]
public class BotController(ILogger<BotController> logger, IOptions<BotConfiguration> Config, ITgUpdateService tgUpdateService) : ControllerBase
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
            {
                tgUpdate model = new tgUpdate();
                model.Id = update.Id;
                model.FBot = "Haich";
                model.FCreateTime = DateTime.Now;
                model.Type = (int)update.Type;
                model.FType = update.Type.ToString();
                model.FJsonStr = JsonStr;
                model.Message = update.Message is null ? "" : JsonConvert.SerializeObject(update.Message);
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
            }
            #endregion

            #region tgMessage
            if (update.Message != null)
            {
                Message message = update.Message;
                tgMessage mMsg = new tgMessage();
                mMsg.FBot = "Haich";
                mMsg.FCreateTime = DateTime.Now;
                mMsg.FType = message.Type.ToString();
                mMsg.FUpdateId = update.Id;
                mMsg.Id = message.Id;
                mMsg.Type = (int)message.Type;
                mMsg.MessageThreadId = message.MessageThreadId;
                mMsg.From = message.From is null ? "" : JsonConvert.SerializeObject(message.From);
                mMsg.SenderChat = message.SenderChat is null ? "" : JsonConvert.SerializeObject(message.SenderChat);
                mMsg.SenderBoostCount = message.SenderBoostCount;
                mMsg.SenderBusinessBot = message.SenderBusinessBot is null ? "" : JsonConvert.SerializeObject(message.SenderBusinessBot);
                mMsg.Date = message.Date;
                mMsg.BusinessConnectionId = message.BusinessConnectionId;
                mMsg.Chat = message.Chat is null ? "" : JsonConvert.SerializeObject(message.Chat);
                mMsg.ForwardOrigin = message.ForwardOrigin is null ? "" : JsonConvert.SerializeObject(message.ForwardOrigin);
                mMsg.IsTopicMessage = message.IsTopicMessage;
                mMsg.IsAutomaticForward = message.IsAutomaticForward;
                mMsg.ReplyToMessage = message.ReplyToMessage is null ? "" : JsonConvert.SerializeObject(message.ReplyToMessage);
                mMsg.ExternalReply = message.ExternalReply is null ? "" : JsonConvert.SerializeObject(message.ExternalReply);

                mMsg.Quote = message.Quote is null ? "" : JsonConvert.SerializeObject(message.Quote);
                mMsg.ReplyToStory = message.ReplyToStory is null ? "" : JsonConvert.SerializeObject(message.ReplyToStory);
                mMsg.ViaBot = message.ViaBot is null ? "" : JsonConvert.SerializeObject(message.ViaBot);
                mMsg.EditDate = message.EditDate;
                mMsg.HasProtectedContent = message.HasProtectedContent;
                mMsg.IsFromOffline = message.IsFromOffline;
                mMsg.MediaGroupId = message.MediaGroupId;
                mMsg.AuthorSignature = message.AuthorSignature;
                mMsg.Text = message.Text;
                mMsg.Entities = message.Entities is null ? "" : JsonConvert.SerializeObject(message.Entities);
                mMsg.LinkPreviewOptions = message.LinkPreviewOptions is null ? "" : JsonConvert.SerializeObject(message.LinkPreviewOptions);
                mMsg.EffectId = message.EffectId;
                mMsg.Animation = message.Animation is null ? "" : JsonConvert.SerializeObject(message.Animation);
                mMsg.Audio = message.Audio is null ? "" : JsonConvert.SerializeObject(message.Audio);
                mMsg.Document = message.Document is null ? "" : JsonConvert.SerializeObject(message.Document);
                mMsg.PaidMedia = message.PaidMedia is null ? "" : JsonConvert.SerializeObject(message.PaidMedia);
                mMsg.Photo = message.Photo is null ? "" : JsonConvert.SerializeObject(message.Photo);
                mMsg.Sticker = message.Sticker is null ? "" : JsonConvert.SerializeObject(message.Sticker);
                mMsg.Story = message.Story is null ? "" : JsonConvert.SerializeObject(message.Story);
                mMsg.Video = message.Video is null ? "" : JsonConvert.SerializeObject(message.Video);
                mMsg.VideoNote = message.VideoNote is null ? "" : JsonConvert.SerializeObject(message.VideoNote);
                mMsg.Voice = message.Voice is null ? "" : JsonConvert.SerializeObject(message.Voice);
                mMsg.Caption = message.Caption;
                mMsg.CaptionEntities = message.CaptionEntities is null ? "" : JsonConvert.SerializeObject(message.CaptionEntities);
                mMsg.ShowCaptionAboveMedia = message.ShowCaptionAboveMedia;
                mMsg.HasMediaSpoiler = message.HasMediaSpoiler;
                mMsg.Contact = message.Contact is null ? "" : JsonConvert.SerializeObject(message.Contact);
                mMsg.Dice = message.Dice is null ? "" : JsonConvert.SerializeObject(message.Dice);
                mMsg.Game = message.Game is null ? "" : JsonConvert.SerializeObject(message.Game);
                mMsg.Poll = message.Poll is null ? "" : JsonConvert.SerializeObject(message.Poll);
                mMsg.Venue = message.Venue is null ? "" : JsonConvert.SerializeObject(message.Venue);
                mMsg.Location = message.Location is null ? "" : JsonConvert.SerializeObject(message.Location);
                mMsg.NewChatMembers = message.NewChatMembers is null ? "" : JsonConvert.SerializeObject(message.NewChatMembers);
                mMsg.LeftChatMember = message.LeftChatMember is null ? "" : JsonConvert.SerializeObject(message.LeftChatMember);
                mMsg.NewChatTitle = message.NewChatTitle;
                mMsg.NewChatPhoto = message.NewChatPhoto is null ? "" : JsonConvert.SerializeObject(message.NewChatPhoto);
                mMsg.DeleteChatPhoto = message.DeleteChatPhoto;
                mMsg.GroupChatCreated = message.GroupChatCreated;
                mMsg.SupergroupChatCreated = message.SupergroupChatCreated;
                mMsg.ChannelChatCreated = message.ChannelChatCreated;
                mMsg.MessageAutoDeleteTimerChanged = message.MessageAutoDeleteTimerChanged is null ? "" : JsonConvert.SerializeObject(message.MessageAutoDeleteTimerChanged);
                mMsg.MigrateToChatId = message.MigrateToChatId;
                mMsg.MigrateFromChatId = message.MigrateFromChatId;
                mMsg.PinnedMessage = message.PinnedMessage is null ? "" : JsonConvert.SerializeObject(message.PinnedMessage);
                mMsg.Invoice = message.Invoice is null ? "" : JsonConvert.SerializeObject(message.Invoice);
                mMsg.SuccessfulPayment = message.SuccessfulPayment is null ? "" : JsonConvert.SerializeObject(message.SuccessfulPayment);
                mMsg.RefundedPayment = message.RefundedPayment is null ? "" : JsonConvert.SerializeObject(message.RefundedPayment);
                mMsg.UsersShared = message.UsersShared is null ? "" : JsonConvert.SerializeObject(message.UsersShared);
                mMsg.ChatShared = message.ChatShared is null ? "" : JsonConvert.SerializeObject(message.ChatShared);
                mMsg.ConnectedWebsite = message.ConnectedWebsite;
                mMsg.WriteAccessAllowed = message.WriteAccessAllowed is null ? "" : JsonConvert.SerializeObject(message.WriteAccessAllowed);
                mMsg.PassportData = message.PassportData is null ? "" : JsonConvert.SerializeObject(message.PassportData);
                mMsg.ProximityAlertTriggered = message.ProximityAlertTriggered is null ? "" : JsonConvert.SerializeObject(message.ProximityAlertTriggered);
                mMsg.BoostAdded = message.BoostAdded is null ? "" : JsonConvert.SerializeObject(message.BoostAdded);
                mMsg.ChatBackgroundSet = message.ChatBackgroundSet is null ? "" : JsonConvert.SerializeObject(message.ChatBackgroundSet);
                mMsg.ForumTopicCreated = message.ForumTopicCreated is null ? "" : JsonConvert.SerializeObject(message.ForumTopicCreated);
                mMsg.ForumTopicEdited = message.ForumTopicEdited is null ? "" : JsonConvert.SerializeObject(message.ForumTopicEdited);
                mMsg.ForumTopicClosed = message.ForumTopicClosed is null ? "" : JsonConvert.SerializeObject(message.ForumTopicClosed);
                mMsg.ForumTopicReopened = message.ForumTopicReopened is null ? "" : JsonConvert.SerializeObject(message.ForumTopicReopened);
                mMsg.GeneralForumTopicHidden = message.GeneralForumTopicHidden is null ? "" : JsonConvert.SerializeObject(message.GeneralForumTopicHidden);
                mMsg.GeneralForumTopicUnhidden = message.GeneralForumTopicUnhidden is null ? "" : JsonConvert.SerializeObject(message.GeneralForumTopicUnhidden);
                mMsg.GiveawayCreated = message.GiveawayCreated is null ? "" : JsonConvert.SerializeObject(message.GiveawayCreated);
                mMsg.Giveaway = message.Giveaway is null ? "" : JsonConvert.SerializeObject(message.Giveaway);
                mMsg.GiveawayWinners = message.GiveawayWinners is null ? "" : JsonConvert.SerializeObject(message.GiveawayWinners);
                mMsg.GiveawayCompleted = message.GiveawayCompleted is null ? "" : JsonConvert.SerializeObject(message.GiveawayCompleted);
                mMsg.VideoChatScheduled = message.VideoChatScheduled is null ? "" : JsonConvert.SerializeObject(message.VideoChatScheduled);
                mMsg.VideoChatStarted = message.VideoChatStarted is null ? "" : JsonConvert.SerializeObject(message.VideoChatStarted);
                mMsg.VideoChatEnded = message.VideoChatEnded is null ? "" : JsonConvert.SerializeObject(message.VideoChatEnded);
                mMsg.VideoChatParticipantsInvited = message.VideoChatParticipantsInvited is null ? "" : JsonConvert.SerializeObject(message.VideoChatParticipantsInvited);
                mMsg.WebAppData = message.WebAppData is null ? "" : JsonConvert.SerializeObject(message.WebAppData);
                mMsg.ReplyMarkup = message.ReplyMarkup is null ? "" : JsonConvert.SerializeObject(message.ReplyMarkup);
                tgUpdateService.Insert(mMsg);
            }
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
