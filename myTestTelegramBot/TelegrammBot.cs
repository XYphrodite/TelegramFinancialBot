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
                        await Start();
                        break;
                    case BotCommands.GetLastFive:
                        await GetLastFive();
                        break;
                    case BotCommands.GetAll:
                        await GetAll();
                        break;
                    case BotCommands.GetToday:
                        await GetToday();
                        break;
                    case BotCommands.GetYestaerday:
                        await GetYesterday();
                        break;
                    case BotCommands.GetWeek:
                        await GetWeek();
                        break;
                    case BotCommands.GetMonth:
                        await GetMonth();
                        break;
                    default: //del
                        {
                            if (message.Text.Contains("/del"))
                            {
                                await Delete();
                                break;
                            }
                            else
                            {
                                await client.SendTextMessageAsync(message.Chat.Id, "Такой команды не найдено!");
                                break;
                            }
                        }
                }



                async Task Start()
                {
                    await client.SendTextMessageAsync(message.Chat.Id, "Приступим, " + message.From.FirstName + ".");
                }
                async Task GetLastFive()
                {
                    user = await _repository.GetUser(message.From.Id);
                    if (user is null)
                    {
                        await client.SendTextMessageAsync(message.Chat.Id, "Ваших данных нет в базе!");
                        return;
                    }
                    var transactions = user.Transactions.OrderBy(t => t.Date).TakeLast(5);
                    string response = GetText(transactions, "Ваши последние пять транзакций:\n", true);
                    await client.SendTextMessageAsync(message.Chat.Id, response);
                }
                async Task GetAll()
                {
                    user = await _repository.GetUser(message.From.Id);
                    if (user is null)
                    {
                        await client.SendTextMessageAsync(message.Chat.Id, "Ваших данных нет в базе!");
                        return;
                    }
                    var transactions = user.Transactions.OrderBy(t => t.Date);
                    string response = GetText(transactions, "Все ваши транзакции:\n");
                    await client.SendTextMessageAsync(message.Chat.Id, response);
                }
                async Task GetToday()
                {
                    user = await _repository.GetUser(message.From.Id);
                    if (user is null)
                    {
                        await client.SendTextMessageAsync(message.Chat.Id, "Ваших данных нет в базе!");
                        return;
                    }
                    var transactions = user.Transactions
                        .OrderBy(t => t.Date)
                        .Where(t => t.Date.Date.ToShortDateString() == message.Date.ToShortDateString());
                    string response = GetText(transactions, "Ваши транзакции за сегодня:\n");
                    await client.SendTextMessageAsync(message.Chat.Id, response);
                } //fix
                async Task GetYesterday()
                {
                    user = await _repository.GetUser(message.From.Id);
                    if (user is null)
                    {
                        await client.SendTextMessageAsync(message.Chat.Id, "Ваших данных нет в базе!");
                        return;
                    }
                    var transactions = user.Transactions
                        .OrderBy(t => t.Date)
                        .Where(t => t.Date.Date >= message.Date.AddDays(-2) && t.Date.Date <= message.Date.AddDays(-1));
                    string response = GetText(transactions, "Ваши транзакции за вчерашний день:\n");
                    await client.SendTextMessageAsync(message.Chat.Id, response);
                } //fix
                async Task GetWeek()
                {
                    user = await _repository.GetUser(message.From.Id);
                    if (user is null)
                    {
                        await client.SendTextMessageAsync(message.Chat.Id, "Ваших данных нет в базе!");
                        return;
                    }
                    var transactions = user.Transactions
                        .OrderBy(t => t.Date)
                        .Where(t => t.Date.Date > message.Date.AddDays(-7) && t.Date.Date <= message.Date);
                    string response = GetText(transactions, "Ваши транзакции за последнюю неделю:\n");
                    await client.SendTextMessageAsync(message.Chat.Id, response);
                }
                async Task GetMonth()
                {
                    user = await _repository.GetUser(message.From.Id);
                    if (user is null)
                    {
                        await client.SendTextMessageAsync(message.Chat.Id, "Ваших данных нет в базе!");
                        return;
                    }
                    var transactions = user.Transactions
                        .OrderBy(t => t.Date)
                        .Where(t => t.Date.Date > message.Date.AddDays(-30) && t.Date.Date <= message.Date);
                    string response = GetText(transactions, "Ваши транзакции за последний месяц:\n");
                    await client.SendTextMessageAsync(message.Chat.Id, response);
                }
                async Task Delete()
                {
                    if (int.TryParse(message.Text.Replace("/del", ""), out int id))
                    {
                        user = await _repository.GetUser(message.From.Id);
                        if (user is null)
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, "Ваших данных нет в базе!");
                            return;
                        }
                        TransactionModel transaction = user.Transactions.FirstOrDefault(t => t.Id == id);
                        if (transaction != null)
                        {
                            await _repository.Delete(transaction);
                            await client.SendTextMessageAsync(message.Chat.Id, "Транзакция успешна удалена!");
                        }
                        else
                            await client.SendTextMessageAsync(message.Chat.Id, "Такой транзакции не найдено!");

                    }
                    else
                        await client.SendTextMessageAsync(message.Chat.Id, "Такой транзакции не найдено!");
                }
            }
            string GetText(IEnumerable<TransactionModel> transactions, string text, bool hasCommand = false)
            {
                if (transactions is null)
                    return "Не найдено!";
                if (!transactions.Any())
                    return "Не найдено!";
                string toReturn = string.Empty;
                foreach (var transaction in transactions)
                {
                    toReturn += transaction.Date.ToShortDateString()
                        + "    "
                        + transaction.Text
                        + (hasCommand is true ? "    /del" + transaction.Id : string.Empty)
                        + Environment.NewLine;
                }
                return text + toReturn;
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
