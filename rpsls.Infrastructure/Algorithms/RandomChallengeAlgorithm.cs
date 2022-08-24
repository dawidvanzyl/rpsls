using rpsls.Domain.Enums;
using rpsls.Domain.Values;
using rpsls.Infrastructure.Algorithms.Contexts;
using System;
using System.Linq;

namespace rpsls.Infrastructure.Algorithms
{
    public class RandomChallengeAlgorithm : AbstractChallengeAlgorithm<AlgorithmContext>
    {
        public RandomChallengeAlgorithm()
        {
        }

        public override ChallengeValue GetChallenge(AlgorithmContext context)
        {
            var winRounds = context.GameRounds
                 .Where(gameRound => gameRound.PlayerTwo.ChallengeResult == ChallengeResult.Won);

            var randomizer = new Random();
            var nextChallenge = randomizer.Next(1, winRounds.Count() - 1);
            var winRound = winRounds.ElementAt(nextChallenge);

            return gameValue.Challenges.First(challenge => challenge.Attack.Value == winRound.PlayerTwo.AttackValue);
        }
    }
}