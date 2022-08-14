using rpsls.Domain.Values;
using rpsls.Infrastructure.Algorithms.Contexts;
using System.Linq;

namespace rpsls.Infrastructure.Algorithms
{
    public interface IChallengeAlgorithm
    {
        void SetupChallenges(GameValue gameValue);
    }

    public interface IChallengeAlgorithm<in TAlgorithmContext> : IChallengeAlgorithm
        where TAlgorithmContext : AlgorithmContext
    {
        ChallengeValue GetChallenge(TAlgorithmContext context);
    }

    public abstract class AbstractChallengeAlgorithm<TAlgorithmContext> : IChallengeAlgorithm<TAlgorithmContext>
        where TAlgorithmContext : AlgorithmContext
    {
        protected ChallengeValue[] challenges;

        protected AbstractChallengeAlgorithm()
        {
        }

        public abstract ChallengeValue GetChallenge(TAlgorithmContext context);

        public virtual void SetupChallenges(GameValue gameValue)
        {
            this.challenges = gameValue.Challenges.ToArray();
        }
    }
}