using rpsls.Domain;
using rpsls.Domain.Enums;
using rpsls.Domain.Values;
using rpsls.Infrastructure.Algorithms;
using rpsls.Infrastructure.Repositories;
using System;

namespace rpsls.Application
{
    public class GameService
    {
        private IAttackStrategy attackStrategy;
        private GameValue gameValue;
        private IMatchRepository matchRepository;

        public GameService(GameValue gameValue, IMatchRepository matchRepository, IAttackStrategyFactory attackStrategyFactory)
        {
            this.gameValue = gameValue;
            this.matchRepository = matchRepository;

            attackStrategy = attackStrategyFactory.CreateRandomAttackAlgorithm(gameValue);
        }

        public Attack GetAttack()
        {
            return attackStrategy.GetAttack();
        }

        public void SaveMatchResult(Attack playerOne, Attack playerTwo, AttackResult attackResult)
        {
            var playerTwoResult = ReverseAttackResult(attackResult);
            var matchResult = new MatchResult
            {
                Id = Guid.NewGuid(),
                PlayerOne = new PlayerResult
                {
                    Id = $"{playerOne.AttackValue.Value}-{attackResult}",
                    PlayerAttack = playerOne.AttackValue,
                    AttackResult = attackResult
                },
                PlayerTwo = new PlayerResult
                {
                    Id = $"{playerTwo.AttackValue.Value}-{playerTwoResult}",
                    PlayerAttack = playerTwo.AttackValue,
                    AttackResult = playerTwoResult
                }
            };

            matchRepository.Add(matchResult);
        }

        private AttackResult ReverseAttackResult(AttackResult attackResult)
        {
            return attackResult switch
            {
                AttackResult.Won => AttackResult.Lost,
                AttackResult.Lost => AttackResult.Won,
                _ => AttackResult.Tied
            };
        }
    }
}