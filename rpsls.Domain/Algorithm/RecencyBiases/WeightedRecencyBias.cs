using Microsoft.Extensions.Logging;
using rpsls.Domain.Algorithms.Contexts;
using rpsls.Domain.Algorithms.RecencyBiases.Abstracts;
using rpsls.Entities.Enums;

namespace rpsls.Domain.Algorithms.RecencyBiases;

public class WeightedRecencyBias(ILogger<WeightedRecencyBias> logger)
    : AbstactRecencyBias(logger)
{
    protected override decimal GetBoostedPercentage(
        AttackTypes attack,
        decimal percentage,
        decimal recencyBiasFactor,
        AlgorithmContext context)
    {
        var recencyBiasBoost = 1 + recencyBiasFactor;
        var boostedPercentage = percentage * recencyBiasBoost;

        logger.LogTrace("Recency Bais Boost: {RecencyBiasBoost}", recencyBiasBoost);
        logger.LogTrace("Boosted Percentage: {BoostedPercentage}", boostedPercentage);

        return boostedPercentage;
    }
}