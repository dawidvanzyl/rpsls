using Microsoft.Extensions.Logging;
using rpsls.Domain.Algorithms.Contexts;
using rpsls.Domain.Algorithms.RecencyBiases.Abstracts;
using rpsls.Entities.Enums;

namespace rpsls.Domain.Algorithms.RecencyBiases
{
    public class NormalizedRecencyBias(ILogger<NormalizedRecencyBias> logger)
        : AbstactRecencyBias(logger)
    {
        protected override decimal GetBoostedPercentage(AttackTypes attack, decimal percentage, decimal recencyBiasFactor, AlgorithmContext context)
        {
            var maxPercentage = 100m; // Assume the percentage is normalized to 0-100
            var normalizedBias = recencyBiasFactor * (percentage / maxPercentage);

            logger.LogDebug("Recency Bias Factor: {RecencyBiasFactor}", recencyBiasFactor);
            logger.LogDebug("Max Percentage: {MaxPercentage}", maxPercentage);
            logger.LogDebug("Normalized Bias: {NormalizedBias}", normalizedBias);

            return normalizedBias;
        }
    }
}