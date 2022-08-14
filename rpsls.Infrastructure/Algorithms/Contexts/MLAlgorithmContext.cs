using rpsls.Domain;
using rpsls.Domain.Enums;
using rpsls.Infrastructure.Algorithms.Models;
using rpsls.Infrastructure.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace rpsls.Infrastructure.Algorithms.Contexts
{
    public class MLAlgorithmContext : AlgorithmContext
    {
        public MLAlgorithmContext(IMatchRepository matchRepository)
            : base(matchRepository)
        {
        }

        public List<MLModel> AIWins { get; private set; }
        public List<MLModel> MLChallengeModels { get; private set; }

        public override void SaveGameRound(GameRound gameRound)
        {
            base.SaveGameRound(gameRound);

            var model = ToMLModel(gameRound);
            MLChallengeModels.Add(model);

            if (gameRound.PlayerTwo.ChallengeResult == ChallengeResult.Won)
            {
                AIWins.Add(model);
            }
        }

        internal override void LoadMatchResults()
        {
            base.LoadMatchResults();

            MLChallengeModels = GameRounds
                .Select(gameRound => ToMLModel(gameRound))
                .ToList();

            AIWins = GameRounds
                .Where(gameRound => gameRound.PlayerTwo.ChallengeResult == ChallengeResult.Won)
                .Select(gameRound => ToMLModel(gameRound))
                .ToList();
        }

        private static MLModel ToMLModel(GameRound gameRound)
        {
            return new MLModel { Player = gameRound.PlayerOne.AttackValue, AI = gameRound.PlayerTwo.AttackValue };
        }
    }
}