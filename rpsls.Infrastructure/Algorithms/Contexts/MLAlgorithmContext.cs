using rpsls.Domain;
using rpsls.Domain.Values;
using rpsls.Infrastructure.Algorithms.Models;
using System.Collections.Generic;
using System.Linq;

namespace rpsls.Infrastructure.Algorithms.Contexts
{
    public class MLAlgorithmContext : AlgorithmContext
    {
        public uint? LastAIChallange { get; private set; }
        public IList<MLModel> MLModels { get; private set; }

        public override void SaveGameRound(GameRound gameRound)
        {
            base.SaveGameRound(gameRound);

            MLModels.Add(MLModel.From(gameRound.GameName, gameRound));

            LastAIChallange = MLModels.Last().AI;
        }

        internal override void LoadPreviousMatches(GameValue gameValue, IList<GameRound> gameRounds)
        {
            base.LoadPreviousMatches(gameValue, gameRounds);

            MLModels = GameRounds
                .Select(gameRound => MLModel.From(gameValue.Name, gameRound))
                .ToList();
        }
    }
}