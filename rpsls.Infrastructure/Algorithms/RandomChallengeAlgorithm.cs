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

            var upscaledChallengeLimit = (winRounds.Count() - 1) * 100;
            var randomizer = new Random();
            var nextChallenge = randomizer.Next(1, upscaledChallengeLimit);
            var downscaledNextChallenge = Convert.ToByte(Math.Truncate(Convert.ToDecimal(nextChallenge) / 100));
            var winRound = winRounds.ElementAt(downscaledNextChallenge);
            return gameValue.Challenges.First(challenge => challenge.Attack.Value == winRound.PlayerTwo.AttackValue);
        }
    }
}