using Microsoft.Extensions.Logging;
using rpsls.Domain.Algorithms.Contexts;
using rpsls.Domain.Algorithms.RecencyBiases.Abstracts;
using rpsls.Entities.Enums;

namespace rpsls.Domain.Algorithms.RecencyBiases
{
    public class RecencyBiasByMatchCount(ILogger<RecencyBiasByMatchCount> logger)
        : AbstactRecencyBias(logger)
    {
        protected override decimal GetBoostedPercentage(
            AttackTypes attack,
            decimal percentage,
            decimal recencyBiasFactor,
            AlgorithmContext context)
        {
            var recentMatchBias = recencyBiasFactor / context.TotalCount;
            var recencyBiasBoost = 1 + recentMatchBias;
            var boostedPercentage = percentage * recencyBiasBoost;

            logger.LogDebug("Recency Bais Boost: {RecencyBiasBoost}", recencyBiasBoost);
            logger.LogDebug("Boosted Percentage: {BoostedPercentage}", boostedPercentage);

            return boostedPercentage;
        }
    }
}