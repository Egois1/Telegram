using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Microsoft.VisualBasic;


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
            var message = update.Message;
            var input = message.Text;
            if (!int.TryParse(input, out int i) || i < 100 || i > 999)
            {
                await botClient.SendTextMessageAsync
                (chatId: message.Chat.Id, text: "Выберите число от 100 до 999.");
                return;
            }
            var url = $"https://http.dog/{i}.webp";
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return;
            Message sentMessage = await botClient.SendPhotoAsync
            (chatId: message.Chat.Id,
            photo: InputFile.FromUri(url),
            caption: "Изображение собаки!",
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
