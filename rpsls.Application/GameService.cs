using rpsls.Domain;
using rpsls.Domain.Enums;
using rpsls.Infrastructure.Repositories;

namespace rpsls.Application
{
    public interface IGameService
    {
        Match CreateMatch(int bestOf);

        Task SaveRoundResultAsync(AttackTypes p1, AttackTypes p2, ResultTypes result);
    }

    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;

        public GameService(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public Match CreateMatch(int bestOf)
        {
            var winningScore = (int)Math.Round(bestOf * 0.66m);
            return new Match(winningScore);
        }

        public async Task SaveRoundResultAsync(AttackTypes p1, AttackTypes p2, ResultTypes result)
        {
            await _gameRepository.CreateMatchResultAsync(p1, p2, result);
        }
    }
}