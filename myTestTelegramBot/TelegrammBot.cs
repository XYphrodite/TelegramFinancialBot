using myTestTelegramBot.Services;
using Telegram.Bot;
using myTestTelegramBot.Models;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TelegramFinanicialBot.Services;
using TelegramFinanicialBot.Consts;

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
                    if (IsCommand())
                    {
                        await DoCommand();
                    }
                    else
                    {
                        await AddTransaction();
                    }
                }
                else if (message.Audio != null)
                {
                    await client.SendTextMessageAsync(message.Chat.Id, "Аудио формат не поддерживается!\nТолько текст.");
                }
                else if (message.Voice != null)
                {
                    await client.SendTextMessageAsync(message.Chat.Id, "Формат голосовых сообщений не поддерживается!\nТолько текст.");
                }
                else if (message.VideoNote != null || message.Video != null)
                {
                    await client.SendTextMessageAsync(message.Chat.Id, "Видео формат не поддерживается!\nТолько текст.");
                }
                else
                {
                    await client.SendTextMessageAsync(message.Chat.Id, "Низвестный формат!");
                }
            }
            return;


            string GetLogMsg() => message.From.Username + ":\t" + message.Text;
            bool IsCommand() => message.Text[0] is '/' ? true : false;
            async Task DoCommand()
            {
                UserModel user;
                switch (message.Text)
                {
                    case BotCommands.Start:
                        await client.SendTextMessageAsync(message.Chat.Id, "Приступим.");
                        break;
                    case BotCommands.GetLastFive:
                        user = await _repository.GetUser(message.From.Id);
                        var lastFive = user.Transactions.OrderBy(t => t.Date).TakeLast(5);
                        await client.SendTextMessageAsync(message.Chat.Id, GetText(lastFive));
                        break;
                    case BotCommands.GetAll:
                        user = await _repository.GetUser(message.From.Id);
                        var transactions = user.Transactions.OrderBy(t => t.Date);
                        await client.SendTextMessageAsync(message.Chat.Id, GetText(transactions));
                        break;
                    case BotCommands.Delete:
                        break;
                    default:
                        await client.SendTextMessageAsync(message.Chat.Id, "Такой команды не найдено!");
                        break;
                }
            }
            string GetText(IEnumerable<TransactionModel> transactions)
            {
                string toReturn = string.Empty;
                foreach (var transaction in transactions)
                {
                    toReturn += transaction.Text + Environment.NewLine;
                }
                return toReturn;
            }
            async Task AddTransaction()
            {
                var transaction = new TransactionModel(message);
                await _repository.Add(transaction);
                await client.SendTextMessageAsync(message.Chat.Id, "Добавлено\n" + message.Text);
            }
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
