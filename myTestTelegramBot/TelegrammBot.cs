using myTestTelegramBot.Services;
using Telegram.Bot;
using myTestTelegramBot.Models;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TelegramFinanicialBot
{
    public class TelegrammBot : ITelegrammBot
    {
        private readonly Repository _repository;
        private readonly IConfiguration _config;
        private readonly ILogger<TelegrammBot> _log;

        public TelegrammBot(Repository repository, IConfiguration config, ILogger<TelegrammBot> log)
        {
            _repository = repository;
            _config = config;
            _log = log;
        }

        public void Run()
        {
            var key = _config.GetValue<string>("TelegrammBotClientId");
            if (string.IsNullOrEmpty(key))
                throw new Exception("TelegrammBotClientId wasn't found!!");

            var client = new TelegramBotClient(key);
            client.StartReceiving(Update, Error);
        }

        private async Task Update(ITelegramBotClient client, Update update, CancellationToken token)
        {
            var message = update.Message;
            if (message != null)
            {
                if (!string.IsNullOrEmpty(message.Text))
                {
                    _log.LogInformation(GetLogMsg());
                    var transaction = new TransactionModel(message);
                    await _repository.Add(transaction);
                    await client.SendTextMessageAsync(message.Chat.Id, "Добавлено\n" + message.Text);
                    return;
                }
            }


            string GetLogMsg() => message.From.Username + ":\t" + message.Text;
        }

        private Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
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
}
