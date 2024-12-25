using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using rpsls.IoC;
using Serilog;

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
        .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));

    private static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration().CreateLogger();

        var host = CreateHostBuilder(args).Build();
        var workerInstance = host.Services.GetRequiredService<HostedService>();
        await workerInstance.Execute();
        await host.RunAsync();
    }
}