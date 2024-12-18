using rpsls.Application;
using rpsls.Domain.Modules;
using rpsls.Entities.Enums;
using Sharprompt;

namespace rpsls.Console
{
    public class HostedService
    {
        private readonly IGameModule _gameModule;
        private readonly IGameService _gameService;

        public HostedService(IGameModule gameModule, IGameService gameService)
        {
            _gameModule = gameModule;
            _gameService = gameService;
        }

        public async Task Execute()
        {
            var p1 = Prompt.Input<string>("Player 1 name");
            var p2 = "Computer";
            var bestOf = Prompt.Input<int>("Best out of");

            _gameModule.Setup();
            var match = _gameService.CreateMatch(bestOf);
            var algorithm = _gameService.GetAlgorithm();

            while (!match.IsOver())
            {
                var p1Attack = Prompt.Select<AttackTypes>($"{p1} attack");
                var p2Attack = algorithm.CalculateAttack();

                System.Console.WriteLine($"{p2} attack: {p2Attack}");
                var result = match.GetResult(p1Attack, p2Attack);

                _gameModule.AddMatchResult(p1Attack, p2Attack, result);

                System.Console.WriteLine(result);
                System.Console.WriteLine();
            }

            await _gameService.SaveMatchResultsAsync();
            var matchScores = match.GetScores();
            System.Console.WriteLine("Game over");
            System.Console.WriteLine($"{p1} - {matchScores[0]} : {p2} - {matchScores[1]}");
        }
    }
}