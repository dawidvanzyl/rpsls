using rpsls.Application;
using rpsls.Domain.Modules;
using rpsls.Entities.Enums;
using Sharprompt;

namespace rpsls.Console;

public class HostedService(IMatchResultModule matchResultModule, IGameService gameService)
{
    public async Task Execute()
    {
        var p1 = Prompt.Input<string>("Player 1 name");
        var p2 = "Computer";
        var bestOf = Prompt.Input<int>("Best out of");

        var match = gameService.CreateMatch(bestOf);
        var algorithm = gameService.GetAlgorithm();

        while (!match.IsOver())
        {
            var p1Attack = Prompt.Select<AttackTypes>($"{p1} attack");
            var p2Attack = algorithm.CalculateAttack();

            System.Console.WriteLine($"{p2} attack: {p2Attack}");
            var result = match.GetResult(p1Attack, p2Attack);

            matchResultModule.Add(p1Attack, p2Attack, result);

            System.Console.WriteLine(result);
            System.Console.WriteLine();
        }

        await gameService.SaveMatchResultsAsync();
        var matchScores = match.GetScores();
        System.Console.WriteLine("Game over");
        System.Console.WriteLine($"{p1} - {matchScores[0]} : {p2} - {matchScores[1]}");
    }
}