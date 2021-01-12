using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using rpsls.Application;
using rpsls.Application.Repositories;
using rpsls.Infrastructure.Factories;
using System.Linq;

namespace rpsls.Console
{
    class Program
    {
        private static ServiceProvider serviceProvider;

        static void Main(string[] args)
        {
            serviceProvider = ServiceCollectionFactory
                .Create()
                .AddSingleton<GamePrompts>()
                .BuildServiceProvider();

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(ProcessArguments);            
        }

        private static void ProcessArguments(Options obj)
        {
            if (obj.Training)
            {
                var gameServiceFactory = serviceProvider.GetService<GameServiceFactory>();
                var appSettings = serviceProvider.GetService<ISettingsRepository>();
                var gameValues = appSettings.GetGameValues();
                var gameValue = gameValues.First(gameValue => gameValue.Name.Equals("Rock-Paper-Scissors-Lizard-Spock"));

                var gameService = gameServiceFactory.Create(gameValue);
                gameService.GenerateTrainingData();
            }
            else
            {
                var gamePrompts = serviceProvider.GetService<GamePrompts>();
                gamePrompts.Play();
            }            
        }
    }
}
