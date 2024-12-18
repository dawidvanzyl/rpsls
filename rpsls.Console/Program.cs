using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using rpsls.IoC;

namespace rpsls.Console;

public static class Program
{
    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .ConfigureServices((_, services) =>
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            services
                .Default()
                .AddSingleton(configuration)
                .AddTransient<HostedService>();
        })
        .ConfigureLogging((_, logging) =>
        {
            logging.ClearProviders();
            logging.AddSimpleConsole(options => options.IncludeScopes = true);
        });

    private static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        var workerInstance = host.Services.GetRequiredService<HostedService>();
        await workerInstance.Execute();
        await host.RunAsync();

        //var serviceProvider = Container
        //    .Create()
        //    .BuildServiceProvider();

        //var gameService = serviceProvider.GetRequiredService<IGameService>();
        //var gameModule = serviceProvider.GetRequiredService<IGameModule>();
    }
}