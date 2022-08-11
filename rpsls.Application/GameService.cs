using rpsls.Domain;
using rpsls.Domain.Enums;
using rpsls.Domain.Values;
using rpsls.Infrastructure.Algorithms;
using rpsls.Infrastructure.Repositories;

namespace rpsls.Application
{
    public class GameService
    {
        private readonly IAttackStrategy attackStrategy;
        private readonly IMatchRepository matchRepository;

        public GameService(GameValue gameValue, IMatchRepository matchRepository, IAttackStrategyFactory attackStrategyFactory)
        {
            this.matchRepository = matchRepository;

            attackStrategy = attackStrategyFactory.CreateMLAttackAlgorithm(gameValue);
        }

        public Attack GetAttack()
        {
            var mlAttackAlgorithm = attackStrategy as MLAttackAlgorithm;
            mlAttackAlgorithm.TrainContext();
            return mlAttackAlgorithm.GetAttack();
        }

        public void SaveMatchResult(Attack playerOne, Attack playerTwo, AttackResult attackResult)
        {
            matchRepository.Add(MatchResult.From(playerOne, playerTwo, attackResult));

            var mlAttackAlgorithm = attackStrategy as MLAttackAlgorithm;
            mlAttackAlgorithm.SavePlayerAttack(playerOne, playerTwo);
        }
    }
}