using rpsls.Domain.Values;
using System;

namespace rpsls.Infrastructure.Algorithms
{
    public class RandomChallangeAlgorithm : AbstractChallangeAlgorithm
    {
        public RandomChallangeAlgorithm(ChallangeValue[] challanges)
            : base(challanges)
        {
        }

        public override ChallangeValue GetChallange()
        {
            var upscaledChallangeLimit = (challanges.Length - 1) * 100;
            var randomizer = new Random();
            var nextChallange = randomizer.Next(1, upscaledChallangeLimit);
            var downscaledNextChallange = Convert.ToByte(Math.Truncate(Convert.ToDecimal(nextChallange) / 100));
            var challange = challanges[downscaledNextChallange];
            return challange;
        }
    }
}