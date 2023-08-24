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
using TelegramFinanicialBot;
using TelegramFinanicialBot.Services;

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
                services.AddSingleton<ITelegrammBot, TelegrammBot>();
            })
            //.UseSerilog()
            .Build();

        var svc = ActivatorUtilities.CreateInstance<TelegrammBot>(host.Services);
        svc.Run();

        while (true)
        {
            string com = Console.ReadLine();
            if (com == "exit") return;
            else Console.WriteLine("Такой команды не найдено");
        }
    }


    private static void BuildConfig(IConfigurationBuilder builder)
    {
        builder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables();
    }
}