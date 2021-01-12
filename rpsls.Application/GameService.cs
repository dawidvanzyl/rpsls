using rpsls.Application.Entities;
using rpsls.Application.Repositories;
using rpsls.Domain;
using rpsls.Domain.Enums;
using rpsls.Domain.Values;
using System.IO;
using System.Linq;

namespace rpsls.Application
{
    public class GameService
    {
        private readonly GameValue gameValue;
        private readonly IMatchRepository matchRepository;
        private readonly IAttackStrategyFactory attackStrategyFactory;

        internal GameService(GameValue gameValue, IMatchRepository matchRepository, IAttackStrategyFactory attackStrategyFactory)
        {
            this.gameValue = gameValue;
            this.matchRepository = matchRepository;
            this.attackStrategyFactory = attackStrategyFactory;
        }

        public Attack PredictAttack(MatchResult previousMatchResult)
        {
            var attackStrategy = previousMatchResult == null
                ? attackStrategyFactory.CreateRandomAttackAlgorithm(gameValue)
                : attackStrategyFactory.CreateMLAttackAlgorithm(gameValue);

            return attackStrategy.GetAttack(previousMatchResult);            
        }

        public MatchResult RecordMatchResult(Attack player1, Attack player2, MatchResult previousMatchResult)
        {
            var attackResult = (AttackResult)player1.CompareTo(player2);

            if (previousMatchResult != null)
            {
                matchRepository.Add(player1, previousMatchResult.AttackResult, previousMatchResult.Player2);
            }            

            return MatchResult.From(player1, player2, attackResult);
        }

        public void GenerateTrainingData()
        {
            if (File.Exists("rpsls.training.csv"))
            {
                File.Delete("rpsls.training.csv");
            }

            using (var fileStream = new FileStream("rpsls.training.csv", FileMode.Create))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.WriteLine("computer,player,result");

                var attacks = gameValue.Attacks.ToArray();
                for (int playerIndex = 0; playerIndex < attacks.Length; playerIndex++)
                {
                    for (int computerIndex = 0; computerIndex < attacks.Length; computerIndex++)
                    {
                        for (int round = 0; round < 20; round++)
                        {
                            var playerAttack = attacks[playerIndex];
                            var computerAttack = attacks[computerIndex];
                            var attackResult = computerAttack.CompareTo(playerAttack);
                            streamWriter.WriteLine($"{playerAttack.AttackValue.Value},{computerAttack.AttackValue.Value},{attackResult}");
                        }
                    }
                }
            }
        }
    }
}
