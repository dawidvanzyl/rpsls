using Microsoft.Extensions.Logging;
using rpsls.Domain.Algorithms.RecencyBiases.Abstracts;
using rpsls.Domain.Modules;
using rpsls.Entities.Enums;

namespace rpsls.Domain.Algorithms.RecencyBiases;

public class WeightedRecencyBias(ILogger<WeightedRecencyBias> logger, IGameModule gameModule)
    : AbstactRecencyBiasAlgorithm(logger, gameModule)
{
    protected override decimal GetBoostedPercentage(AttackTypes attack, decimal percentage, decimal recencyBiasFactor)
    {
        var recencyBiasBoost = 1 + recencyBiasFactor;
        var boostedPercentage = percentage * recencyBiasBoost;

        logger.LogTrace("Recency Bais Boost: {RecencyBiasBoost}", recencyBiasBoost);
        logger.LogTrace("Boosted Percentage: {BoostedPercentage}", boostedPercentage);

        return boostedPercentage;
    }
}