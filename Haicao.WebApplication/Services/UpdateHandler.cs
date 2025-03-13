using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace Haicao.WebApplication.Services;

public class UpdateHandler(ITelegramBotClient bot, ILogger<UpdateHandler> logger) : IUpdateHandler
{
    private static readonly InputPollOption[] PollOptions = ["Hello", "World!"];

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        logger.LogInformation("HandleError: {Exception}", exception);
        // Cooldown in case of network connection error
        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await (update switch
        {
            // 当用户发送一条新消息时触发。
            { Message: { } message } => OnMessage(message),
            // 当用户编辑一条已发送的消息时触发。
            { EditedMessage: { } message } => OnMessage(message),
            // 表示一个回调查询（Callback Query）。当用户点击一个内联键盘按钮时触发。
            { CallbackQuery: { } callbackQuery } => OnCallbackQuery(callbackQuery),
            // 表示一个内联查询（Inline Query）。当用户发起一个内联查询时触发。
            { InlineQuery: { } inlineQuery } => OnInlineQuery(inlineQuery),
            // 表示用户选择的内联查询结果。当用户从内联查询结果中选择一个选项时触发。(未成功触发）
            { ChosenInlineResult: { } chosenInlineResult } => OnChosenInlineResult(chosenInlineResult),
            // 表示一个投票（Poll）。投票状态发生变化时触发。(匿名投票才会触发）
            { Poll: { } poll } => OnPoll(poll),
            // 表示用户对投票的答案。当用户参与投票时触发。
            { PollAnswer: { } pollAnswer } => OnPollAnswer(pollAnswer),
            
            // ChannelPost: // 当频道发布一条新消息时触发。
            // EditedChannelPost: // 当频道中的一条消息被编辑时触发。

            // ShippingQuery: // 表示一个配送查询（Shipping Query）。当用户填写配送信息时触发。
            // PreCheckoutQuery: // 表示一个预结账查询（Pre-Checkout Query）。当用户选择一个商品并发送付款时触发。

            // MyChatMember: // 表示机器人在聊天中的成员状态更新。当机器人在聊天中的权限或状态发生变化时触发。
            // 表示聊天成员的更新。当聊天中的某个成员的权限或状态发生变化时触发。
            //{ ChatMember : { } chatMember } => UnknownUpdateHandlerAsync(chatMember),
            // ChatJoinRequest 表示用户请求加入聊天。当用户请求加入频道或群组时触发。

            //// Connect 子业务账户 连接变更
            //{ BusinessConnection: { } dt } => OtherUpdateHandler(dt),
            //// 来自 Connect 子业务账户的新消息
            //{ BusinessMessage: { } dt } => OtherUpdateHandler(dt),
            //{ EditedBusinessMessage: { } dt } => OtherUpdateHandler(dt),
            //{ DeletedBusinessMessages: { } dt } => OtherUpdateHandler(dt),

            //// 用户更改了对消息的回应
            //{ MessageReaction: { } dt } => OtherUpdateHandler(dt),
            //// 更改了对带有匿名反应的消息的回应。机器人必须是聊天中的管理员，并且必须在allowed_updates列表中明确指定接收这些更新
            //{ MessageReactionCount: { } dt } => OtherUpdateHandler(dt),

            //// 用户购买了由机器人在非渠道聊天中发送的非空负载的付费媒体
            //{ PurchasedPaidMedia: { } dt } => OtherUpdateHandler(dt),

            //// 添加或修改了chat boost
            //{ ChatBoost: { } dt } => OtherUpdateHandler(dt),
            //{ RemovedChatBoost: { } dt } => OtherUpdateHandler(dt),

            // 其他 或 未知的Update
            _ => UnknownUpdateHandlerAsync(update)
        });
    }

    #region 测试命令
    private async Task OnMessage(Message msg)
    {
        logger.LogInformation("Receive message type: {MessageType}", msg.Type);
        if (msg.Text is not { } messageText)
            return;

        Message sentMessage = await (messageText.Split(' ')[0] switch
        {
            "/photo" => SendPhoto(msg),
            "/inline_buttons" => SendInlineKeyboard(msg),
            "/keyboard" => SendReplyKeyboard(msg),
            "/remove" => RemoveKeyboard(msg),
            "/request" => RequestContactAndLocation(msg),
            "/inline_mode" => StartInlineQuery(msg),
            "/poll" => SendPoll(msg),
            "/poll_anonymous" => SendAnonymousPoll(msg),
            "/throw" => FailingHandler(msg),
            _ => Usage(msg)
        });
        logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.Id);
    }

    async Task<Message> Usage(Message msg)
    {
        const string usage = """
                <b><u>Bot menu</u></b>:
                /photo          - send a photo
                /inline_buttons - send inline buttons
                /keyboard       - send keyboard buttons
                /remove         - remove keyboard buttons
                /request        - request location or contact
                /inline_mode    - send inline-mode results list
                /poll           - send a poll
                /poll_anonymous - send an anonymous poll
                /throw          - what happens if handler fails
            """;
        return await bot.SendMessage(msg.Chat, usage, parseMode: ParseMode.Html, replyMarkup: new ReplyKeyboardRemove());
    }

    async Task<Message> SendPhoto(Message msg)
    {
        await bot.SendChatAction(msg.Chat, ChatAction.UploadPhoto);
        await Task.Delay(2000); // simulate a long task
        await using var fileStream = new FileStream("Files/bot.gif", FileMode.Open, FileAccess.Read);
        return await bot.SendPhoto(msg.Chat, fileStream, caption: "Read https://telegrambots.github.io/book/");
    }

    // Send inline keyboard. You can process responses in OnCallbackQuery handler
    async Task<Message> SendInlineKeyboard(Message msg)
    {
        return await bot.SendMessage(msg.Chat, "Inline buttons:", replyMarkup: new InlineKeyboardButton[][] {
                ["1.1", "1.2", "1.3"],
                [("WithCallbackData", "CallbackData"), ("WithUrl", "https://github.com/TelegramBots/Telegram.Bot")]
            });
    }

    async Task<Message> SendReplyKeyboard(Message msg)
    {
        return await bot.SendMessage(msg.Chat, "Keyboard buttons:", replyMarkup: new string[][] { ["1.1", "1.2", "1.3"], ["2.1", "2.2"] });
    }

    async Task<Message> RemoveKeyboard(Message msg)
    {
        return await bot.SendMessage(msg.Chat, "Removing keyboard", replyMarkup: new ReplyKeyboardRemove());
    }

    async Task<Message> RequestContactAndLocation(Message msg)
    {
        var replyMarkup = new ReplyKeyboardMarkup(true)
            .AddButton(KeyboardButton.WithRequestLocation("Location"))
            .AddButton(KeyboardButton.WithRequestContact("Contact"));
        return await bot.SendMessage(msg.Chat, "Who or Where are you?", replyMarkup: replyMarkup);
    }

    async Task<Message> StartInlineQuery(Message msg)
    {
        var button = InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Inline Mode");
        return await bot.SendMessage(msg.Chat, "Press the button to start Inline Query\n\n" +
            "(Make sure you enabled Inline Mode in @BotFather)", replyMarkup: new InlineKeyboardMarkup(button));
    }

    async Task<Message> SendPoll(Message msg)
    {
        return await bot.SendPoll(msg.Chat, "Question", PollOptions, isAnonymous: false);
    }

    async Task<Message> SendAnonymousPoll(Message msg)
    {
        return await bot.SendPoll(chatId: msg.Chat, "Question", PollOptions);
    }

    static Task<Message> FailingHandler(Message msg)
    {
        throw new NotImplementedException("FailingHandler");
    }
    #endregion

    // Process Inline Keyboard callback data
    private async Task OnCallbackQuery(CallbackQuery callbackQuery)
    {
        logger.LogInformation("Received inline keyboard callback from: {CallbackQueryId}", callbackQuery.Id);
        await bot.AnswerCallbackQuery(callbackQuery.Id, $"Received {callbackQuery.Data}");
        await bot.SendMessage(callbackQuery.Message!.Chat, $"Received {callbackQuery.Data}");
    }

    #region Inline Mode

    private async Task OnInlineQuery(InlineQuery inlineQuery)
    {
        logger.LogInformation("Received inline query from: {InlineQueryFromId}", inlineQuery.From.Id);

        InlineQueryResult[] results = [ // displayed result
            new InlineQueryResultArticle("1", "Telegram.Bot", new InputTextMessageContent("hello")),
            new InlineQueryResultArticle("2", "is the best", new InputTextMessageContent("world"))
        ];
        await bot.AnswerInlineQuery(inlineQuery.Id, results, cacheTime: 0, isPersonal: true);
    }

    private async Task OnChosenInlineResult(ChosenInlineResult chosenInlineResult)
    {
        logger.LogInformation("Received inline result: {ChosenInlineResultId}", chosenInlineResult.ResultId);
        await bot.SendMessage(chosenInlineResult.From.Id, $"You chose result with Id: {chosenInlineResult.ResultId}");
    }

    #endregion

    private Task OnPoll(Poll poll)
    {
        logger.LogInformation("Received Poll info: {Question}", poll.Question);
        return Task.CompletedTask;
    }

    private async Task OnPollAnswer(PollAnswer pollAnswer)
    {
        var answer = pollAnswer.OptionIds.FirstOrDefault();
        logger.LogInformation("Received Poll info: {U}={A}", pollAnswer.User.Id, answer);
        var selectedOption = PollOptions[answer];
        if (pollAnswer.User != null)
            await bot.SendMessage(pollAnswer.User.Id, $"You've chosen: {selectedOption.Text} in poll");
    }

    private Task UnknownUpdateHandlerAsync(Update update)
    {
        logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }
}
