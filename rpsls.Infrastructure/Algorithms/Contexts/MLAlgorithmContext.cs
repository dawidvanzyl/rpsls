using rpsls.Domain;
using rpsls.Domain.Enums;
using rpsls.Domain.Values;
using rpsls.Infrastructure.Algorithms.Models;
using System.Collections.Generic;
using System.Linq;

namespace rpsls.Infrastructure.Algorithms.Contexts
{
    public class MLAlgorithmContext : AlgorithmContext
    {
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

        internal override void LoadPreviousMatches(GameValue gameValue, IList<GameRound> gameRounds)
        {
            base.LoadPreviousMatches(gameValue, gameRounds);

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
            return new MLModel { GameName = gameRound.GameName, Player = gameRound.PlayerOne.AttackValue, AI = gameRound.PlayerTwo.AttackValue };
        }
    }
}