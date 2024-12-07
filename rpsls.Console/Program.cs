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

        var player1 = Prompt.Input<string>("Player 1 name");
        var player2 = "Computer 1";

        var player1Attack = Prompt.Select<AttackTypes>($"{player1} attack");
        var player2Attack = (AttackTypes)Random.Shared.Next(1, 3);

        System.Console.WriteLine($"{player2} attack: {player2Attack}");

        var result = gameService
            .GetMatchResultAsync(player1Attack, player2Attack)
            .GetAwaiter()
            .GetResult();
        System.Console.WriteLine(result);
    }
}