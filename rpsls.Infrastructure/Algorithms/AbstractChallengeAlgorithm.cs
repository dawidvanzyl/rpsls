using rpsls.Domain.Values;
using rpsls.Infrastructure.Algorithms.Contexts;

namespace rpsls.Infrastructure.Algorithms
{
    public interface IChallengeAlgorithm
    {
        void SetupChallenges(ChallengeValue[] challenges);
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

        public virtual void SetupChallenges(ChallengeValue[] challenges)
        {
            this.challenges = challenges;
        }
    }
}