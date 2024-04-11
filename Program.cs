using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;


internal class Program
{
    static async Task Main(string[] args)
    {
        var botClient = new TelegramBotClient("6932018755:AAGdjlI43JCETcm_HBnLkK_OMgyXXNzs5tM");

        var me = await botClient.GetMeAsync();
        Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");

        using CancellationTokenSource cts = new();

        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        Console.ReadLine();
        cts.Cancel();

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            InlineKeyboardMarkup inlineKeyboard = new(new[]
 {
    InlineKeyboardButton.WithUrl(
        text: "Link to the Repository",
        url: "https://github.com/TelegramBots/Telegram.Bot")
});

            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "A message with an inline keyboard markup",
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
        }
    

        Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}