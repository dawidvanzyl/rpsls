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
                 .Where(gameRound => gameRound.PlayerTwo.ChallengeResult == ChallengeResult.Won)
                 .Take(gameValue.Challenges.Count);

            ChallengeValue challange = null;
            while (challange == null)
            {
                var randomizer = new Random();
                var nextChallenge = randomizer.Next(1, winRounds.Count());
                var winRound = winRounds.ElementAt(nextChallenge - 1);

                challange = gameValue.Challenges.FirstOrDefault(challenge => challenge.Attack.Value == winRound.PlayerTwo.AttackValue);
            }

            return challange;
        }
    }
}