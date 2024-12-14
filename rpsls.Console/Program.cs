using Microsoft.Extensions.DependencyInjection;
using rpsls.Application;
using rpsls.Domain.Enums;
using rpsls.IoC;
using Sharprompt;

namespace rpsls.Console;

public static class Program
{
    private static void Main()
    {
        var serviceProvider = Container
            .Create()
            .BuildServiceProvider();

        var gameService = serviceProvider.GetRequiredService<IGameService>();

        var p1 = Prompt.Input<string>("Player 1 name");
        var p2 = "Computer";
        var bestOf = Prompt.Input<int>("Best out of");
        var match = gameService.CreateMatch(bestOf);

        while (!match.IsOver())
        {
            var p1Attack = Prompt.Select<AttackTypes>($"{p1} attack");
            var p2Attack = (AttackTypes)Random.Shared.Next(1, 3);

            System.Console.WriteLine($"{p2} attack: {p2Attack}");

            var result = match.GetResult(p1Attack, p2Attack);

            gameService
                .SaveRoundResultAsync(p1Attack, p2Attack, result)
                .GetAwaiter()
                .GetResult();

            System.Console.WriteLine(result);
            System.Console.WriteLine();
        }

        var matchScores = match.GetScores();
        System.Console.WriteLine("Game over");
        System.Console.WriteLine($"{p1} - {matchScores[0]} : {p2} - {matchScores[1]}");
    }
}