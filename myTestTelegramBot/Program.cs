using Telegram.Bot;
using Telegram.Bot.Types;

internal class Program
{
    private static void Main(string[] args)
    {
        var client = new TelegramBotClient("6382152721:AAEqD1HF0Ai6H5ObJptgk9PGCd8DoGasf28");
        client.StartReceiving(Update, Error);
        Console.ReadLine();
    }

    private static async Task Update(ITelegramBotClient client, Update update, CancellationToken token)
    {
        var message = update.Message;
        if(message != null)
        {
            if (message.Text.ToLower().Contains("привет"))
            {
                await client.SendTextMessageAsync(message.Chat.Id, message.Text);
                return;
            }
        }
    }

    private static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}