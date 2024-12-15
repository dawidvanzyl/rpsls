using Microsoft.Extensions.DependencyInjection;
using rpsls.Application;
using rpsls.Domain.Modules;
using rpsls.Entities.Enums;
using rpsls.IoC;
using Sharprompt;

namespace rpsls.Console;

public static class Program
{
    private static async Task Main()
    {
        var serviceProvider = Container
            .Create()
            .BuildServiceProvider();

        var gameService = serviceProvider.GetRequiredService<IGameService>();
        var gameModule = serviceProvider.GetRequiredService<IGameModule>();

        var p1 = Prompt.Input<string>("Player 1 name");
        var p2 = "Computer";
        var bestOf = Prompt.Input<int>("Best out of");

        gameModule.Setup();
        var match = gameService.CreateMatch(bestOf);
        var algorithm = gameService.GetAlgorithm();

        while (!match.IsOver())
        {
            var p1Attack = Prompt.Select<AttackTypes>($"{p1} attack");
            var p2Attack = algorithm.CalculateAttack();

            System.Console.WriteLine($"{p2} attack: {p2Attack}");

            var result = match.GetResult(p1Attack, p2Attack);

            gameModule.AddMatchResult(p1Attack, p2Attack, result);

            System.Console.WriteLine(result);
            System.Console.WriteLine();
        }

        await gameService.SaveMatchResultsAsync();
        var matchScores = match.GetScores();
        System.Console.WriteLine("Game over");
        System.Console.WriteLine($"{p1} - {matchScores[0]} : {p2} - {matchScores[1]}");
    }
}