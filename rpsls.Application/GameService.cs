using rpsls.Domain;
using rpsls.Domain.Algorithms;
using rpsls.Domain.Modules;

namespace rpsls.Application
{
    public interface IGameService
    {
        Match CreateMatch(int bestOf);

        IAlgorithm GetAlgorithm();

        Task SaveMatchResultsAsync();
    }

    public class GameService : IGameService
    {
        private readonly IAlgorithm _algorithm;
        private readonly IGameModule _gameModule;

        public GameService(IGameModule gameModule, IAlgorithm algorithm)
        {
            _gameModule = gameModule;
            _algorithm = algorithm;
        }

        public Match CreateMatch(int bestOf)
        {
            var winningScore = (int)Math.Round(bestOf * 0.66m);
            return new Match(winningScore);
        }

        public IAlgorithm GetAlgorithm()
        {
            _algorithm.SetupRuleSet();
            return _algorithm;
        }

        public async Task SaveMatchResultsAsync()
        {
            await _gameModule.SaveMatchResultsAsync();
        }
    }
}