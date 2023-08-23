using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using myTestTelegramBot.Data;
using myTestTelegramBot.Models;
using myTestTelegramBot.Services;
using Serilog;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

internal class Program
{
    private static void Main(string[] args)
    {
        var buiulder = new ConfigurationBuilder();
        BuildConfig(buiulder);

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(buiulder.Build())
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        Log.Logger.Information("Application Starting");

        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddDbContext<ApplicationContext>();
                services.AddTransient<Repository>();
            })
            //.UseSerilog()
            .Build();


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
                await Repository.Add(transaction);
                await client.SendTextMessageAsync(message.Chat.Id, "Добавлено\n"+ message.Text);
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


    private static void BuildConfig(IConfigurationBuilder builder)
    {
        builder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables();
    }
}