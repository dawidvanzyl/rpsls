using rpsls.Entities;
using rpsls.Entities.Enums;
using rpsls.Infrastructure.Repositories;

namespace rpsls.Domain.Modules
{
    public interface IGameModule
    {
        void AddMatchResult(AttackTypes p1, AttackTypes p2, ResultTypes result);

        IList<MatchResult> GetMatchResults();

        Task SaveMatchResultsAsync();

        void Setup();
    }

    public class GameModule : IGameModule
    {
        private readonly IGameRepository _gameRepository;
        private Lazy<IList<MatchResult>> _matchResults;

        public GameModule(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public void AddMatchResult(AttackTypes p1, AttackTypes p2, ResultTypes result)
        {
            _matchResults.Value.Add(MatchResult.Create(p1, p2, result));
        }

        public IList<MatchResult> GetMatchResults()
        {
            return _matchResults.Value;
        }

        public async Task SaveMatchResultsAsync()
        {
            await _gameRepository
                .CreateMatchResultsAsync(
                    _matchResults
                        .Value
                        .Where(matchResult => matchResult.IsNew));
        }

        public void Setup()
        {
            _matchResults = new Lazy<IList<MatchResult>>(
                () => _gameRepository
                        .GetMatchResultsAsync()
                        .GetAwaiter()
                        .GetResult());
        }
    }
}