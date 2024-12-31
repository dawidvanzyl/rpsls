using Microsoft.Extensions.Logging;
using rpsls.Domain.Algorithms.Contexts;
using rpsls.Domain.Algorithms.RecencyBiases.Abstracts;
using rpsls.Entities.Enums;

namespace rpsls.Domain.Algorithms.RecencyBiases
{
    public class ExponentialDecayRecencyBias(ILogger<ExponentialDecayRecencyBias> logger)
        : AbstactRecencyBias(logger)
    {
        protected override decimal GetBoostedPercentage(
            AttackTypes attack,
            decimal percentage,
            decimal recencyBiasFactor,
            AlgorithmContext context)
        {
            var attackMatches = context.MatchHistory
                .Where(match => match.P1Attack == attack)
                .ToList();

            var roundsSinceLastMatch = attackMatches[^1].Round - attackMatches[^2].Round; // Number of matches since this attack was used
            var decayRate = 0.1m; // Lambda for exponential decay
            var decayedBias = recencyBiasFactor * (decimal)Math.Exp((double)(-decayRate * roundsSinceLastMatch));
            var boostedPercentage = percentage + decayedBias;

            logger.LogDebug("Decay Rate: {DecayRate}", decayRate);
            logger.LogDebug("Decayed Bias: {DecayedBias}", decayedBias);
            logger.LogDebug("Boosted Percentage: {BoostedPercentage}", boostedPercentage);

            return boostedPercentage;
        }
    }
}