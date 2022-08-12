using rpsls.Domain.Values;

namespace rpsls.Infrastructure.Algorithms
{
    public interface IChallangeAlgorithm
    {
        ChallangeValue GetChallange();
    }

    public abstract class AbstractChallangeAlgorithm : IChallangeAlgorithm
    {
        protected readonly ChallangeValue[] challanges;

        protected AbstractChallangeAlgorithm(ChallangeValue[] challanges)
        {
            this.challanges = challanges;
        }

        public abstract ChallangeValue GetChallange();
    }
}