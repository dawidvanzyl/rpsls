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
        public IList<MLChallengeModel> MLChallengeModels { get; private set; }
        public IList<MLPlayerAttackModel> MLPlayerAttackModels { get; private set; }

        public override void SaveGameRound(GameRound gameRound)
        {
            base.SaveGameRound(gameRound);

            //if (gameRound.PlayerOne.ChallengeResult is ChallengeResult.Won or ChallengeResult.Tied)
            {
                MLPlayerAttackModels.Add(MLPlayerAttackModel.From(gameRound.GameName, gameRound.PlayerOne));
            }

            if (gameRound.PlayerTwo.ChallengeResult == ChallengeResult.Won)
            {
                MLChallengeModels.Add(MLChallengeModel.From(gameRound.GameName, gameRound));
            }
        }

        internal override void LoadPreviousMatches(GameValue gameValue, IList<GameRound> gameRounds)
        {
            base.LoadPreviousMatches(gameValue, gameRounds);

            MLPlayerAttackModels = GameRounds
                //.Where(gameRound => gameRound.PlayerOne.ChallengeResult is ChallengeResult.Won or ChallengeResult.Tied)
                .Select(gameRound => MLPlayerAttackModel.From(gameValue.Name, gameRound.PlayerOne))
                .ToList();

            MLChallengeModels = GameRounds
                .Where(gameRound => gameRound.PlayerTwo.ChallengeResult == ChallengeResult.Won)
                .Select(gameRound => MLChallengeModel.From(gameValue.Name, gameRound))
                .ToList();
        }
    }
}