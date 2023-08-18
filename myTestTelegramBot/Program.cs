using myTestTelegramBot.Models;
using myTestTelegramBot.Services;
using System.Text.Encodings.Web;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

internal class Program
{
    static JsonSerializerOptions options = new JsonSerializerOptions
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true
    };
    private static void Main(string[] args)
    {
        var client = new TelegramBotClient("6382152721:AAEqD1HF0Ai6H5ObJptgk9PGCd8DoGasf28");
        client.StartReceiving(Update, Error);
        Console.ReadLine();
    }

    private static async Task Update(ITelegramBotClient client, Update update, CancellationToken token)
    {
        var message = update.Message;
        if (message != null)
        {
            if (!string.IsNullOrEmpty(message.Text))
            {
                var transaction = new TransactionModel(message);
                GoogleWorker.Add(transaction);
                var json = JsonSerializer.Serialize(transaction, options);
                await client.SendTextMessageAsync(message.Chat.Id, json);
                return;
            }
        }
    }

    private static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}