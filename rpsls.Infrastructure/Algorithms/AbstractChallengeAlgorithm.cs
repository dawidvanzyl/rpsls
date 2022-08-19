using rpsls.Domain.Values;
using rpsls.Infrastructure.Algorithms.Contexts;

namespace rpsls.Infrastructure.Algorithms
{
    public interface IChallengeAlgorithm
    {
        void SetGameValue(GameValue gameValue);
    }

    public interface IChallengeAlgorithm<in TAlgorithmContext> : IChallengeAlgorithm
        where TAlgorithmContext : AlgorithmContext
    {
        ChallengeValue GetChallenge(TAlgorithmContext context);
    }

    public abstract class AbstractChallengeAlgorithm<TAlgorithmContext> : IChallengeAlgorithm<TAlgorithmContext>
        where TAlgorithmContext : AlgorithmContext
    {
        protected GameValue gameValue;

        protected AbstractChallengeAlgorithm()
        {
        }

        public abstract ChallengeValue GetChallenge(TAlgorithmContext context);

        public virtual void SetGameValue(GameValue gameValue)
        {
            this.gameValue = gameValue;
        }
    }
}