using rpsls.Application;
using rpsls.Entities.Enums;
using Sharprompt;

namespace rpsls.Console;

public class HostedService(IGameService gameService, IAttackPredictor attackPredictor)
{
    public async Task Execute()
    {
        var p1 = Prompt.Input<string>("Player 1 name");
        var p2 = "Computer";
        var bestOf = Prompt.Input<int>("Best out of");

        var game = gameService.Create(bestOf);
        while (!gameService.IsOver())
        {
            var p1Attack = Prompt.Select<AttackTypes>($"{p1} attack");
            var p2Attack = attackPredictor.PredictNextAttack();

            System.Console.WriteLine($"{p2} attack: {p2Attack}");
            var result = gameService.GetResult(p1Attack, p2Attack);
            game.AddRound(p1Attack, p2Attack, result);

            System.Console.WriteLine(result);
            System.Console.WriteLine();
        }

        await gameService.SaveAsync();
        System.Console.WriteLine("Matches saved");

        var matchScores = gameService.GetScores();
        System.Console.WriteLine("Game over");
        System.Console.WriteLine($"{p1} - {matchScores[0]} : {p2} - {matchScores[1]}");
    }
}