using rpsls.Domain.Enums;
using rpsls.Infrastructure.Repositories;

namespace rpsls.Application
{
    public interface IGameService
    {
        Task<MatchResultTypes> GetMatchResultAsync(AttackTypes p1, AttackTypes p2);
    }

    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;

        public GameService(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public async Task<MatchResultTypes> GetMatchResultAsync(AttackTypes p1, AttackTypes p2)
        {
            var matchResult = (p1, p2) switch
            {
                _ when p1 == p2 => MatchResultTypes.Draw,
                (AttackTypes.Rock, AttackTypes.Scissor)
                or (AttackTypes.Paper, AttackTypes.Rock)
                or (AttackTypes.Scissor, AttackTypes.Paper) => MatchResultTypes.Win,
                _ => MatchResultTypes.Loss
            };

            await _gameRepository.CreateMatchResultAsync(p1, p2, matchResult);

            return matchResult;
        }
    }
}