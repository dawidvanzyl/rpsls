using rpsls.Domain;
using rpsls.Domain.Enums;
using rpsls.Domain.Values;
using System.Collections.Generic;
using System.Linq;

namespace rpsls.Infrastructure.Algorithms.Contexts
{
    public class AlgorithmContext
    {
        public IList<GameRound> GameRounds { get; private set; }

        public GameValue GameValue { get; private set; }

        public virtual void SaveGameRound(GameRound gameRound)
        {
            GameRounds.Add(gameRound);
        }

        internal virtual void LoadPreviousMatches(GameValue gameValue, IList<GameRound> gameRounds)
        {
            GameValue = gameValue;

            GameRounds = gameValue.Challenges
                .SelectMany(player => gameValue.Challenges
                .Select(machine => GameRound.From(gameValue, player, machine, (ChallengeResult)player.CompareTo(machine))))
                .ToList();

            (GameRounds as List<GameRound>).AddRange(gameRounds);
        }
    }
}